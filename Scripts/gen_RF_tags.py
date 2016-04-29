from sys import argv

filename = argv[1]

tags = [
    ('@PART[*]:HAS[@MODULE[ModuleEngineConfigs]:HAS[@CONFIG[*]:HAS[@PROPELLANT[',
        {
            'LqdOxygen' : ['lox', 'liquid', 'o2', 'cryo'],
            'Kerosene' : ['rp-1', 'rp1', 't-1', 't1', 'rg-1', 'rg1',
                          'hydrocarbon'],
            'LqdHydrogen' : ['liquid', 'h2', 'cryo'],
            'NTO' : ['nitro', 'tetroxide', 'n2o4'],
            'UDMH' : ['unsymmetrical', 'methyl', 'hydrazine'],
            'Hydrazine' : ['n2h4'],
            'Aerozine50' : ['hydrazine', 'unsymmetrical', 'methyl', 'udmh'],
            'MMH' : ['mono', 'methyl', 'hydrazine'],
            'HTP' : ['high', 'test', 'peroxide', 'hydrogen', 'h2o2'],
            'AvGas' : ['plane', 'spirit', 'piston'],
            'IRFNA-III' : ['inhibit', '?red', 'fuming', 'nitric', 'acid', '?3'],
            'NitrousOxide' : ['no2', '?nos'],
            'Aniline' : [],
            'Ethanol75' : ['alcohol'],
            'LqdAmmonia' : ['liquid', 'nh3'],
            'LqdMethane' : ['liquid', 'ch4'],
            'ClF3' : ['chlorine', 'trifluori'],
            'ClF5' : ['chlorine', 'pentafluori'],
            'Diborane' : ['boron'],
            'Pentaborane' : ['boron'],
            'Ethane' : [],
            'Ethylene' : [],
            'OF2' : ['oxygen', 'difluor'],
            'LqdFluorine' : ['liquid'],
            'N2F4' : ['tetra', 'fluoro', 'hydrazine'],
            'Methanol' : ['alcohol'],
            'Furfuryl' : ['alcohol'],
            'UH25' : ['udmh', 'hydrazine'],
            'Tonka250' : ['triethylamine'],
            'Tonka500' : ['triethylamine'],
            'FLOX30' : ['fluorine', 'oxygen'],
            'FLOX70' : ['fluorine', 'oxygen'],
            'FLOX88' : ['fluorine', 'oxygen'],
            'IWFNA' : ['inhibit', 'white', 'fuming', 'nitric', 'acid',
                       'wfna'],
            'IRFNA-IV' : ['inhibit', '?red', 'fuming', 'nitric', 'acid', '?4'],
            'AK20' : ['nitric', 'acid'],
            'AK27' : ['nitric', 'acid'],
            'CaveaB' : [],
            'MON1' : ['mixed', 'oxides', 'nitrogen'],
            'MON3' : ['mixed', 'oxides', 'nitrogen'],
            'MON10' : ['mixed', 'oxides', 'nitrogen'],
            'MON15' : ['mixed', 'oxides', 'nitrogen'],
            'MON20' : ['mixed', 'oxides', 'nitrogen'],
            'MON25' : ['mixed', 'oxides', 'nitrogen'],
            'ArgonGas' : ['noble'],
            'KryptonGas' : ['noble'],
            'Hydyne' : ['udmh', 'unsymmetrical', 'methyl', 'hydrazine', 'deta',
                        'ethylene', 'amine'],
            'Syntin' : ['hydrocarbon'],
            'XenonGas' : ['noble'],
            'EnrichedUranium' : [],
            'TEATEB' : ['ethyl', 'alumin', 'borane'],
            'HTPB' : ['hydroxyl', 'terminat', 'polybutadiene'],
            'PBAN' : ['polybutadiene', 'acrylonitrile'],
            'PSPC' : [],
            'HNIW' : ['hexanitrohexaazaisowurtzitane', 'cl20', 'cl-20'],
            'NGNC' : ['nitroglycerin', 'nitrocellulose']
        }),
    ('@PART[*]:HAS[@MODULE[ModuleFuelTanks]:HAS[type[',
        {
            'Balloon' : [],
            'BalloonCryo' : ['(insulat'],
            'Cryogenic' : ['(insulat'],
            'Default' : [],
            'ElectricPropulsion' : [],
            'Fuselage' : [],
            'LifeSupport' : [],
            'LifeSupportWaste' : [],
            'ServiceModule' : ['pressur', 'insulat', 'cryogenic', 'electri',
                               'life', 'support'],
            'Structural' : []
        })
]

output_file = open(filename, 'w')
output_file.truncate()

for string, tag_dict in tags:
    for k, v in tag_dict.viewitems():
        mm_string = string + k + ']]]]'
        new_tags = ':$: ' + k + ' ' + ' '.join(v) + ':'
        output_file.write(mm_string)
        output_file.write('\n{\n')
        output_file.write('\t@tags ^=' + new_tags)
        output_file.write('\n}\n')
