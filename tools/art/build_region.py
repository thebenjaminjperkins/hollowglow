#!/usr/bin/env python3
"""Reproducible source extraction, palette/density normalization and modular edge atlases."""
import argparse,hashlib,io,json
from pathlib import Path
from PIL import Image,ImageEnhance,ImageOps,ImageFilter
ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'assets/region';SRC=OUT/'source'
PALETTE=json.loads((ROOT/'assets/sanctuary/manifest.json').read_text())['palette']

def quantize(im):
    im=im.convert('RGBA');pal=Image.new('P',(1,1));pal.putpalette([c for rgb in PALETTE*4 for c in bytes.fromhex(rgb)])
    alpha=im.getchannel('A').point(lambda a:255 if a>=128 else 0)
    result=im.convert('RGB').quantize(palette=pal,dither=Image.Dither.NONE).convert('RGBA');result.putalpha(alpha);return result

def png(im):
    data=io.BytesIO();im.save(data,format='PNG');return data.getvalue()

def outputs():
    terrain=Image.open(SRC/'terrain.png').convert('RGBA');props=Image.open(SRC/'props.png').convert('RGBA')
    result={};manifest={'native_pixels_per_world_unit':2,'palette':PALETTE,'sprites':{}}
    specs=[('tree',0,0,(144,208),(132,196),(72,202)),('ruin',1,0,(104,72),(96,64),(52,66)),
           ('corner',2,0,(104,104),(96,96),(52,98)),('sign',0,1,(48,72),(40,64),(24,66)),
           ('deck',1,1,(104,64),(96,56),(52,58)),('mouth',2,1,(160,136),(152,128),(80,130))]
    cuts={}
    for name,col,row,size,limit,anchor in specs:
        cell=props.crop((col*props.width//3+4,row*props.height//2+4,(col+1)*props.width//3-4,(row+1)*props.height//2-4))
        # Preserve generated alpha, remove only its semitransparent source halo.
        cell.putalpha(cell.getchannel('A').point(lambda a:255 if a>=128 else 0))
        bounds=cell.getbbox();assert bounds,name
        cell=cell.crop(bounds);cuts[name]=cell
        ratio=min(limit[0]/cell.width,limit[1]/cell.height);fitted=cell.convert('RGBa').resize((round(cell.width*ratio),round(cell.height*ratio)),Image.Resampling.LANCZOS).convert('RGBA')
        sprite=Image.new('RGBA',size);sprite.alpha_composite(fitted,(anchor[0]-fitted.width//2,anchor[1]-fitted.height));sprite=quantize(sprite)
        result[name+'.png']=png(sprite);manifest['sprites'][name]={'file':name+'.png','size':size,'anchor':anchor}
    samples=[]
    for col,row in [(0,0),(1,0),(0,1),(1,1)]:
        cell=terrain.crop((col*terrain.width//2+12,row*terrain.height//2+12,(col+1)*terrain.width//2-12,(row+1)*terrain.height//2-12)).convert('RGB')
        cell=ImageEnhance.Contrast(cell).enhance(.32 if row==0 and col==0 else .65 if row==0 else .45 if col==0 else .65)
        samples.append(cell.resize((128,128),Image.Resampling.LANCZOS).convert('RGBA'))
    samples.append(ImageEnhance.Brightness(samples[1]).enhance(.82))
    # Normalize the plank density with the deck sprite before extracting a material.
    # Never stretch a small decorative crop across an eight-tile patch.
    deck=cuts['deck'].resize((96,49),Image.Resampling.LANCZOS).crop((32,20,64,36))
    wood=Image.new('RGBA',(64,32));wood.paste(deck,(0,0));wood.paste(ImageOps.mirror(deck),(32,0))
    wood.paste(ImageOps.flip(deck),(0,16));wood.paste(ImageOps.flip(ImageOps.mirror(deck)),(32,16))
    wood_patch=Image.new('RGBA',(128,128))
    for yy in range(0,128,32):
        for xx in range(0,128,64):wood_patch.paste(wood,(xx,yy))
    samples.append(wood_patch)
    # Eight-cell material periods preserve organic clusters; phase follows world coordinates.
    atlas=Image.new('RGBA',(2048,3072))
    import math
    for material,sample in enumerate(samples):
        patch=Image.new('RGBA',(256,256))
        patch.paste(sample,(0,0));patch.paste(ImageOps.mirror(sample),(128,0));patch.paste(ImageOps.flip(sample),(0,128));patch.paste(ImageOps.flip(ImageOps.mirror(sample)),(128,128));patch=quantize(patch)
        for phase in range(64):
            tile=patch.crop((phase%8*32,phase//8*32,phase%8*32+32,phase//8*32+32))
            for mask in range(16):
                edge=tile.copy();alpha=edge.getchannel('A');pixels=alpha.load()
                for y in range(32):
                    for x in range(32):
                        # Coherent, clustered edge erosion; no random per-tile noise or dark grid borders.
                        limit=6+round(2*math.sin((x+phase%8*32)*.19)+2*math.cos((y+phase//8*32)*.17))
                        if mask&1 and y<limit or mask&2 and x>=32-limit or mask&4 and y>=32-limit or mask&8 and x<limit:pixels[x,y]=0
                edge.putalpha(alpha);index=(material*64+phase)*16+mask;atlas.paste(edge,(index%64*32,index//64*32))
    result['terrain.png']=png(atlas);manifest['terrain']={'cell':32,'materials':['grass','road','water','gravel','soil','bridge'],'phases':64,'edge_masks':16,'columns':64}
    manifest['sources']={name:hashlib.sha256((SRC/(name+'.png')).read_bytes()).hexdigest() for name in ['terrain','props']}
    result['manifest.json']=(json.dumps(manifest,indent=2)+'\n').encode();return result

def main():
    parser=argparse.ArgumentParser();parser.add_argument('--check',action='store_true');args=parser.parse_args()
    for name,data in outputs().items():
        path=OUT/name
        if args.check:assert path.read_bytes()==data,name
        else:path.write_bytes(data)
    print('PASS: reference-derived modular region exports, shared palette, native density and deterministic atlas processing')
if __name__=='__main__':main()
