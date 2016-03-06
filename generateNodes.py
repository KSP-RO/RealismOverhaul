#!/usr/bin/env python
import os
import re
import fnmatch
import argparse

parser = argparse.ArgumentParser(description='Generate Rescaled Attach Nodes.')

parser.add_argument("-p", "--path", help="KSP or Gamedata Directory. (Defaults to Current Directory)", default=".")
parser.add_argument("cfgfile", nargs='?', help="File to Write the New Scaled Nodes Into.")
args = parser.parse_args()

def getnodes(path):
    """Function to get cfg file list"""
    for root, dirs, files in os.walk(path):
        for name in files:
            if fnmatch.fnmatch(name, '*.cfg'):
                with open(os.path.join(root, name), 'r') as f:
                    for line in f:
                        if re.match("^node*", line):
                            node = line.split(' ',1)[0]
                            yield node

def writenodecfg(path):
	cfg = header
	with open((args.cfgfile), "w") as out:
		for i in sorted(list(set(getnodes(path)))):
			cfg += body.format(i)
		out.write(cfg)

header = """RESCALEFACTOR
{
	rescaleFactor = 1.6
}
"""

body = """
@PART[*]:HAS[@MODEL,#{0}[*],!MODULE[ProceduralFairing*],!MODULE[KzThrustPlateResizer]]:FOR[zzzzzzROMini]:NEEDS[!RealismOverhaul]
{{
//         0    1    2    3     4    5    6
//$node = 0.0, 0.0, 0.0, 0.0, -1.0, 0.0, 1
//We only need to adjust the first three values...
nxa = #${0}[0]$
nya = #${0}[1]$
nza = #${0}[2]$
@nxa *= #$@RESCALEFACTOR/rescaleFactor$
@nya *= #$@RESCALEFACTOR/rescaleFactor$
@nza *= #$@RESCALEFACTOR/rescaleFactor$
//And then reassemble the node definition.
@{0} = #$nxa$,$nya$,$nza$,${0}[3]$,${0}[4]$,${0}[5]$,${0}[6]$
//This one is curious. The last key defines the size of the green node, and is optional.
//However, the code as written here barfs if the last key is optioned out. A trailing comma is left behind, and partloader says "**** that!"
//I am attemting to strip out the trailing comma and replace it with a whitespace. (That didn't work so well, so maybe we can set it to "1")
@{0}[5] ^= :,$:,1:
//clean up our mess...
!n*a = DEL
}}
"""
if args.cfgfile:
	writenodecfg(args.path)
	print(("Sucess! {} written!").format(args.cfgfile))
else:
	parser.print_help()
