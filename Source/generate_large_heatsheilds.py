output_sizes = (7,8,9,10,12,15,20)
for output_size in output_sizes:
    skip_count = 0
    print 'running for '+str(output_size)
    source = open('1M_HeatShield.cfg')
    output_file = open(str(output_size)+'M_HeatShield.cfg', 'w')
    for line in source:
        if skip_count > 0:
            skip_count = skip_count - 1
            continue
        line = line.replace('mass = 0.04', 'mass = ' + str(0.04*output_size))
        line = line.replace('Heatshield-1M', 'Heatshield-'+ str(output_size) + 'M')
        line = line.replace('Heatshield (1M)', 'Heatshield ('+ str(output_size) + 'M)')
        scale_factor = output_size / 1.25
        reciprocal = 1.0 / scale_factor
        line = line.replace('rescaleFactor = 0.8', 'rescaleFactor = ' + str(scale_factor))
        sr = str(reciprocal)
        line = line.replace('scale = 1.25, 1.25, 1.25', 'scale = %s, %s, %s' % (sr,sr,sr))
        stripped_source = line.strip()
        if stripped_source == 'loss':
            skip_count = 5
            line = 'loss\n{\nkey = 650 0\nkey = 2000 '+str(120*output_size)+'\nkey = 6000 ' + str(150*output_size) + '\n}\n'
        if stripped_source == 'dissipation':
            skip_count = 4
            line = 'dissipation\n{\nkey = 300 0\nkey = 800 '+str(int(720/output_size))+'\n}\n'
        line = line.replace('amount = 250', 'amount = '+str(250*output_size))
        line = line.replace('maxAmount = 250', 'maxAmount = '+str(250*output_size))
        line = line.replace('node_stack_top = 0.0, 0.06196643, 0.0, 0.0, 1.0, 0.0, 1', 'node_stack_top = 0.0, 0.06196643, 0.0, 0.0, 1.0, 0.0, ' + str(output_size))
        line = line.replace('node_stack_bottom = 0.0, -0.01, 0.0, 0.0, 1.0, 0.0, 1', 'node_stack_bottom = 0.0, -0.01, 0.0, 0.0, 1.0, 0.0, ' + str(output_size))
        output_file.write(line)
    output_file.close()
