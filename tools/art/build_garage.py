#!/usr/bin/env python3
"""Reference-derived garage PNGs. Shared density/palette, repeatable extraction."""
import argparse,hashlib,io,json
from pathlib import Path
from PIL import Image,ImageEnhance,ImageOps
ROOT=Path(__file__).resolve().parents[2]
OUT=ROOT/'assets/garage';SRC=OUT/'source'
PALETTE=json.loads((ROOT/'assets/sanctuary/manifest.json').read_text())['palette']
def normalize(im):
    im=im.convert('RGBA');alpha=im.getchannel('A').point(lambda a:255 if a>=128 else 0)
    pal=Image.new('P',(1,1));pal.putpalette([v for h in PALETTE*4 for v in bytes.fromhex(h)])
    im=im.convert('RGB').quantize(palette=pal,dither=Image.Dither.NONE).convert('RGBA');im.putalpha(alpha);return im
def png(im):
    f=io.BytesIO();im.save(f,format='PNG');return f.getvalue()
def exports():
    props=Image.open(SRC/'props.png').convert('RGBA');van=Image.open(SRC/'van.png').convert('RGBA')
    def cell(im,col,row,cols,rows):
        im=im.crop((col*im.width//cols+3,row*im.height//rows+3,(col+1)*im.width//cols-3,(row+1)*im.height//rows-3))
        im.putalpha(im.getchannel('A').point(lambda a:255 if a>=128 else 0));return im.crop(im.getbbox())
    outputs={};manifest={'native_pixels_per_world_unit':2,'palette':PALETTE,'sprites':{}}
    groups=[('cupboard',[(0,0),(1,0),(2,0)],(72,88),(36,84)),
            ('fuel-cabinet',[(0,1),(1,1),(2,1)],(72,96),(36,92)),
            ('crate',[(0,2),(1,2),(2,2)],(64,64),(32,60)),
            ('rack',[(3,0),(3,1)],(64,56),(32,52)),
            ('compressor',[(3,2),(0,3)],(80,72),(40,68)),
            ('wall',[(1,3)],(64,64),(32,60)),('wall-post',[(2,3)],(32,72),(16,68)),
            ('corner',[(3,3)],(64,72),(32,68))]
    for name,coords,size,anchor in groups+[('van',[(0,0),(1,0),(2,0)],(128,192),(64,184))]:
        frames=[cell(van,c,r,3,1) if name=='van' else cell(props,c,r,4,4) for c,r in coords]
        ratio=min((size[0]-8)/max(im.width for im in frames),(size[1]-8)/max(im.height for im in frames))
        sheet=Image.new('RGBA',(size[0]*len(frames),size[1]))
        for i,im in enumerate(frames):
            im=im.convert('RGBa').resize((round(im.width*ratio),round(im.height*ratio)),Image.Resampling.LANCZOS).convert('RGBA')
            sheet.alpha_composite(im,(i*size[0]+anchor[0]-im.width//2,anchor[1]-im.height))
        outputs[name+'.png']=png(normalize(sheet));manifest['sprites'][name]={'size':size,'anchor':anchor,'columns':len(frames)}
    # Native icon extracted from the visible tool on the rack, not a substitute glyph.
    rack=cell(props,3,0,4,4);tool=rack.crop((int(rack.width*.16),int(rack.height*.12),int(rack.width*.39),int(rack.height*.92)))
    tool.thumbnail((20,32),Image.Resampling.LANCZOS);icon=Image.new('RGBA',(32,40));icon.alpha_composite(tool,((32-tool.width)//2,(40-tool.height)//2))
    outputs['tool-icon.png']=png(normalize(icon))
    src=Image.open(SRC/'floor.png').convert('RGBA');atlas=Image.new('RGBA',(512,512))
    for i in range(4):
        col,row=i%2,i//2
        patch=src.crop((col*src.width//2+12,row*src.height//2+12,(col+1)*src.width//2-12,(row+1)*src.height//2-12))
        patch=ImageEnhance.Contrast(patch).enhance(.65).resize((128,128),Image.Resampling.LANCZOS)
        period=Image.new('RGBA',(256,256));period.paste(patch,(0,0));period.paste(ImageOps.mirror(patch),(128,0))
        period.paste(ImageOps.flip(patch),(0,128));period.paste(ImageOps.flip(ImageOps.mirror(patch)),(128,128))
        atlas.paste(normalize(period),(col*256,row*256))
    outputs['floor.png']=png(atlas)
    manifest['sources']={p.name:hashlib.sha256(p.read_bytes()).hexdigest() for p in SRC.glob('*.png')}
    outputs['manifest.json']=(json.dumps(manifest,indent=2)+'\n').encode();return outputs
def main():
    p=argparse.ArgumentParser();p.add_argument('--check',action='store_true');a=p.parse_args()
    for name,data in exports().items():
        if a.check:assert (OUT/name).read_bytes()==data,name
        else:(OUT/name).write_bytes(data)
    print('PASS: deterministic garage PNGs, common palette and 2px/world density')
if __name__=='__main__':main()
