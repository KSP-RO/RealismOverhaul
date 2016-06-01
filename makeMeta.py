import os, argparse, sys, json, glob

class DefaultHelpParser(argparse.ArgumentParser):
    def error(self, message):
        sys.stderr.write('error: %s\n' % message)
        self.print_help()
        sys.exit(2)

HELP_DESC = "Creates neccesary metadata files"
parser = DefaultHelpParser(description=HELP_DESC)
parser.add_argument('tag', metavar='tag', type=str, nargs=1,
                   help='tag of release (e.g. 0.4.6.0')

args = parser.parse_args()

if not args.tag or len(args.tag) < 1:
    print "ERROR: git tag must be specified and must be in the format major.minor.patch.build-configuration.e.g. 0.4.6.0"
    sys.exit(2)

version = args.tag[0].split("v")[1].split(".")
major = int(version[0])
minor = int(version[1])
patch = int(version[2])
build = 0
if len(version) == 4:
	build = int(version[3])
# create AVC .version file
avc = {
	"NAME" : "RealismOverhaul",
	"URL" : "https://raw.githubusercontent.com/NathanKell/RealismOverhaul/master/RealismOverhaul/RO.version",
	"DOWNLOAD" : "https://github.com/NathanKell/RealismOverhaul/releases",
     "GITHUB":
     {
         "USERNAME":"NathanKell",
         "REPOSITORY":"RealismOverhaul",
         "ALLOW_PRE_RELEASE": False
     },
	"VERSION" :
	{
		"MAJOR" : major,
		"MINOR" : minor,
		"PATCH" : patch,
		"BUILD" : build
	},
	"KSP_VERSION" :
	{
		"MAJOR" : 1,
		"MINOR" : 1,
		"PATCH" : 2
	}
}
with open("RO.version", "w") as f:
	f.write(json.dumps(avc))
