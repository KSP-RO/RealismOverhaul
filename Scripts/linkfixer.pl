#!/usr/bin/perl

use strict;
use warnings;
use Cwd;

sub dirscan;
sub filefix;

#Need to descend into every directory beneath us and fix every file.  May as
#well clean up tabs too.

my $start = getcwd;
dirscan($start);



sub dirscan
{
    my $dir = $_[0];
    print "$dir\n";
    opendir (my $DIR, "$dir");
    while (my $in = readdir ($DIR))
    {
        next if ($in =~ /^\./);
        my $this = $dir."/".$in;
        if (-d $this)
        {
            print "\n=======================================================\n";
            print "Descending: $this\n";
            dirscan($this);
        }
        elsif ((-f $this) && ($this =~ /\.cfg$/))
        {
            print "-> Fixing Links:  $this\n";
            filefix($this);
        }
        else
        {
            print "-> Ignoring File: $this\n";
        }
    }
    closedir $DIR;
}
sub filefix
{
    my $file = $_[0];
    open my $INFILE, "<$file" or die "Could not open $file\n";
    my @lines;
    my $i = 0;
    while (my $line = <$INFILE>)
    {
        $line =~ s/RealismOverhaul\/SmokeScreen_MP_Nazari_FX/RealPlume\/MP_Nazari_FX/g;
        $line =~ s/RealismOverhaul\/SmokeScreen_Ferram_FX/RealPlume\/Ferram_FX/g;
        $line =~ s/RealismOverhaul\/SmokeScreen_RE_Sounds/RealPlume\/KW_Sounds/g;
        $lines[$i] = $line;
        $i++;
    }
    close $INFILE;
    open my $OUTFILE, ">$file" or die "Could not open $file\n";
    foreach (@lines) { print $OUTFILE $_; }
    close $OUTFILE;
}