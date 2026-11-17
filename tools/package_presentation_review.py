#!/usr/bin/env python3
"""Archive PresentationReview captures and encode short native-resolution movement comparisons."""
import argparse,json,shutil,subprocess
from pathlib import Path

ROOT=Path(__file__).resolve().parents[1]
def main():
    parser=argparse.ArgumentParser();parser.add_argument('captures',type=Path);args=parser.parse_args()
    measures=ROOT/'docs/measurements/m6c';measures.mkdir(parents=True,exist_ok=True)
    videos=ROOT/'docs/recordings/m6c';videos.mkdir(parents=True,exist_ok=True)
    counts={}
    for tag in ('before','after'):
        folder=args.captures/tag;target=ROOT/'docs/screenshots/m6c'/tag;target.mkdir(parents=True,exist_ok=True)
        for p in folder.glob('*.png'):shutil.copyfile(p,target/p.name)
        shutil.copyfile(folder/'evidence.json',measures/(tag+'.json'))
        for route in ('sanctuary-motion','garage-motion'):
            count=len(list((folder/route).glob('*.png')));assert count>10
            counts[tag+'-'+route]=count
            subprocess.run(['ffmpeg','-y','-loglevel','error','-framerate','15','-i',str(folder/route/'%04d.png'),
                '-c:v','libx264','-threads','1','-preset','fast','-crf','17','-pix_fmt','yuv420p','-movflags','+faststart',str(videos/(tag+'-'+route+'.mp4'))],check=True)
    for route in ('sanctuary-motion','garage-motion'):
        assert counts['before-'+route]==counts['after-'+route]
        subprocess.run(['ffmpeg','-y','-loglevel','error','-i',str(videos/('before-'+route+'.mp4')),'-i',str(videos/('after-'+route+'.mp4')),
            '-filter_complex','[0:v][1:v]hstack=inputs=2[out]','-map','[out]','-c:v','libx264','-threads','1','-preset','fast','-crf','17','-pix_fmt','yuv420p','-movflags','+faststart',str(videos/(route+'-comparison.mp4'))],check=True)
    (measures/'recordings.json').write_text(json.dumps({'frames':counts,'playbackFps':15,'comparisonOrder':'Before left; after right','nativePanelPixels':[1536,864],
        'note':'Actual physics/input, sampled during cell waypoint traversal; short review clips, not real-time performance or expedition timing evidence. Side-by-side panels are not resized. MP4 is lossy; PNGs are the pixel comparison reference.'},indent=2)+'\n')
    print('PASS: matched native-size PNGs and six review clips packaged')
if __name__=='__main__':main()
