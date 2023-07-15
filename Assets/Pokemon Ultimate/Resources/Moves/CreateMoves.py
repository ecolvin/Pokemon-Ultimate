#Copy the header of a move asset to a new file
#Change m_name in the header (line 13)
#Fill out the rest of the new file
#Use a separate program to update all the .meta files with the correct 

with open('Moves.csv', 'r') as input:
    with open('Output.csv', 'w') as output:
        for line in input:
            fields = line.split('|')
            name = fields[1]
            moveType = fields[2]
            moveCat = fields[3]
            pp = fields[4]
            power = fields[5]
            accuracy = fields[6]
            priority = getPriority(name)
            critRate = getCritRate(name)
            range = getRange(name)

            flags = determineFlags(name)
                output.write(name + '|') #Write out all relevant data that will be used to create the Unity Asset


def getPriority(name):
    return 0

def getCritRate(name):
    return 0.0417

def getRange(name):
    return 0

def getRecoilPercent(name):
    return 0

def getSecondaryEffect(name):
    return ""

def getEffectChance(name):
    return 0

def determineFlags(name):
    flags = {}
    flags.add(true)
    return flags


