#!/usr/bin/perl
# config splitter
use strict;
use warnings;

my $name = "VSR";
my $dir = "VenStockRevamp/";
my $bulkfile = "$name.cfg";

open (my $file, "<".$dir.$bulkfile);

my $opencurl = 0;
my $closecurl = 0;

my $partfile;
open ($partfile, ">null.txt");

while (my $line = <$file>)
{
    print $line;
    if ($line =~ /^\@PART/)
    {
        my ($FN) = $line =~ /\@PART\[(.+?)\]/;
        $FN =~ s/\W//g;
        close $partfile;
        open ($partfile, ">".$dir.$FN.".cfg");

    }
    print $partfile $line;
}
unlink "null.txt";
close $file;
rename $dir.$bulkfile, $dir.$bulkfile.".bak";