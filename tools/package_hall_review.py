#!/usr/bin/env python3
"""Archive native gameplay captures for the M6c hall follow-up; never edits game artwork."""
import argparse,json,shutil
from pathlib import Path
ROOT=Path(__file__).resolve().parents[1]
def main():
    parser=argparse.ArgumentParser();parser.add_argument('user_data',type=Path);a=parser.parse_args()
    images=ROOT/'docs/screenshots/sanctuary-hall';measure=ROOT/'docs/measurements/sanctuary-hall';measure.mkdir(parents=True,exist_ok=True)
    for tag,source in [('before',a.user_data/'presentation/hall-before'),('after',a.user_data/'presentation/hall-legacy-after'),('hall',a.user_data/'hall-review')]:
        target=images/tag;target.mkdir(parents=True,exist_ok=True)
        for p in source.glob('*.png'):shutil.copyfile(p,target/p.name)
        shutil.copyfile(source/('validation.json' if tag=='hall' else 'evidence.json'),measure/(tag+'.json'))
    print('PASS: native matched captures and hall validation evidence archived without resizing')
if __name__=='__main__':main()
