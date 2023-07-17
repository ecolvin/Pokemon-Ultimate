#Copy the header of a move asset to a new file
#Change m_name in the header (line 13)
#Fill out the rest of the new file
#Use a separate program to update all the .meta files with the correct 
from enum import Enum

Types = Enum('Types', ['Normal', 'Fire', 'Water', 'Electric', 'Grass', 'Ice', 'Fighting', 'Poison', 'Ground', 'Flying', 'Psychic', 'Bug', 'Rock', 'Ghost', 'Dragon', 'Dark', 'Steel', 'Fairy'])
Categories = Enum('Categories', ['???', 'Status', 'Physical', 'Special'])

def getTypeID(type):
    if (type == "None"):
        return 0
    return Types[type].value

def getCatID(cat):
    return (Categories[cat].value) - 1

def cleanPower(name, power):    
    varPowMoves = ["Acid Downpour", "All-Out Pummeling", "Beat Up", "Black Hole Eclipse", "Bloom Doom", 
        "Breakneck Blitz", "Continental Crush", "Corkscrew Crash", "Crush Grip", "Devastating Drake", 
        "Dragon Energy", "Electro Ball", "Eruption", "Flail", "Fling", "Frustration", "G-Max Befuddle", 
        "G-Max Cannonade", "G-Max Centiferno", "G-Max Chi Strike", "G-Max Cuddle", "G-Max Depletion", 
        "G-Max Finale", "G-Max Foam Burst", "G-Max Gold Rush", "G-Max Gravitas", "G-Max Malodor", 
        "G-Max Meltdown", "G-Max One Blow", "G-Max Rapid Flow", "G-Max Replenish", "G-Max Resonance", 
        "G-Max Sandblast", "G-Max Smite", "G-Max Snooze", "G-Max Steelsurge", "G-Max Stonesurge", 
        "G-Max Stun Shock", "G-Max Sweetness", "G-Max Tartness", "G-Max Terror", "G-Max Vine Lash", 
        "G-Max Volcalith", "G-Max Volt Crash", "G-Max Wildfire", "G-Max Wind Rage", "Gigavolt Havoc", 
        "Grass Knot", "Gyro Ball", "Heat Crash", "Heavy Slam", "Hydro Vortex", "Inferno Overdrive", 
        "Low Kick", "Magnitude", "Max Airstream", "Max Darkness", "Max Flare", "Max Flutterby", "Max Geyser", 
        "Max Hailstorm", "Max Knuckle", "Max Lightning", "Max Mindstorm", "Max Ooze", "Max Overgrowth", 
        "Max Phantasm", "Max Quake", "Max Rockfall", "Max Starfall", "Max Steelspike", "Max Strike", 
        "Max Wyrmwind", "Natural Gift", "Never-Ending Nightmare", "Pika Papow", "Power Trip", "Present", 
        "Psywave", "Punishment", "Return", "Reversal", "Savage Spin-Out", "Shattered Psyche", "Spit Up", 
        "Stored Power", "Subzero Slammer", "Supersonic Skystrike", "Tectonic Rage", "Trump Card", 
        "Twinkle Tackle", "Veevee Volley", "Water Spout", "Wring Out"]

    if(not power.isnumeric()):
        if(varPowMoves.count(name) != 0):
            return -1
        else:
            return 0
    return power


def cleanAccuracy(acc):
    acc = acc.replace("%", "")
    if(acc == "—"):
        return -1
    return acc

def getPriority(name):
    if(["Helping Hand"].count(name) != 0):
        return 5
    if(["Baneful Bunker", "Detect", "Endure", "King's Shield", "Magic Coat", "Max Guard", "Obstruct", "Protect", "Silk Trap", "Snatch", "Spiky Shield"].count(name) != 0):
        return 4
    if(["Crafty Shield", "Fake Out", "Quick Guard", "Spotlight", "Wide Guard"].count(name) != 0):
        return 3
    if(["Ally Switch", "Extreme Speed", "Feint", "First Impression", "Follow Me", "Rage Powder"].count(name) != 0):
        return 2
    if(["Accelerock", "Aqua Jet", "Baby-Doll Eyes", "Bide", "Bullet Punch", "Ice Shard", "Ion Deluge", "Jet Punch", "Mach Punch", "Powder", "Quick Attack", "Shadow Sneak", "Sucker Punch", "Vacuum Wave", "Water Shuriken"].count(name) != 0):
        return 1
    if(["Vital Throw"].count(name) != 0):
        return -1
    if(["Beak Blast", "Focus Punch", "Shell Trap"].count(name) != 0):
        return -3
    if(["Avalanche", "Revenge"].count(name) != 0):
        return -4
    if(["Counter", "Mirror Coat"].count(name) != 0):
        return -5
    if(["Circle Throw", "Dragon Tail", "Roar", "Whirlwind"].count(name) != 0):
        return -6
    if(["Trick Room"].count(name) != 0):
        return -7  
    return 0

def getCritRate(name, moveCat):
    noCrit = ["Bide", "Comeuppance", "Counter", "Dragon Rage", "Endeavor", "Final Gambit", 
        "Guardian of Alola", "Metal Burst", "Mirror Coat", "Nature's Madness", "Night Shade", 
        "Psywave", "Ruination", "Seismic Toss", "Sonic Boom", "Super Fang", 
        "Fissure", "Guillotine", "Horn Drill", "Sheer Cold"]
    highCrit = ["Aeroblast", "Air Cutter", "Aqua Cutter", "Attack Order", "Blaze Kick", 
        "Crabhammer", "Cross Chop", "Cross Poison", "Dire Claw", "Drill Run", "Esper Wing", 
        "Karate Chop", "Leaf Blade", "Night Slash", "Poison Tail", "Psycho Cut", "Razor Leaf", 
        "Razor Wind", "Shadow Blast", "Shadow Claw", "Sky Attack", "Slash", "Snipe Shot", 
        "Spacial Rend", "Stone Edge", "Triple Arrows"]
    alwaysCrit = ["Storm Throw", "Frost Breath", "Zippy Zap", "Surging Strikes", "Wicked Blow", "Flower Trick"]

    if(moveCat == 1 or noCrit.count(name) != 0):
        return -1
    if(highCrit.count(name) != 0):
        return 1
    if(name == "10,000,000 Volt Thunderbolt"):
        return 2
    if(alwaysCrit.count(name) != 0):
        return 3
    return 0

def getRange(name):
    adjacentFoe = ["Doodle", "G-Max Befuddle", "G-Max Cannonade", "G-Max Centiferno", "G-Max Chi Strike", 
        "G-Max Cuddle", "G-Max Depletion", "G-Max Drum Solo", "G-Max Finale", "G-Max Fireball", 
        "G-Max Foam Burst", "G-Max Gold Rush", "G-Max Gravitas", "G-Max Hydrosnipe", "G-Max Malodor", 
        "G-Max Meltdown", "G-Max One Blow", "G-Max Rapid Flow", "G-Max Replenish", "G-Max Resonance", 
        "G-Max Sandblast", "G-Max Smite", "G-Max Steelsurge", "G-Max Stonesurge", "G-Max Stun Shock", 
        "G-Max Sweetness", "G-Max Tartness", "G-Max Terror", "G-Max Vine Lash", "G-Max Volcalith", 
        "G-Max Volt Crash", "G-Max Wildfire", "G-Max Wind Rage", "Max Airstream", "Max Darkness", 
        "Max Flare", "Max Flutterby", "Max Geyser", "Max Hailstorm", "Max Knuckle", "Max Lightning", 
        "Max Mindstorm", "Max Ooze", "Max Overgrowth", "Max Phantasm", "Max Quake", "Max Rockfall", 
        "Max Starfall", "Max Steelspike", "Max Strike", "Max Wyrmwind","Me First"]
    anyOther = ["Acrobatics", "Aerial Ace", "Aeroblast", "Air Slash", "Aura Sphere", "Bounce", "Brave Bird", 
        "Chatter", "Dark Pulse", "Dragon Pulse", "Drill Peck", "Fly", "Flying Press", "Gust", "Heal Pulse", 
        "Hurricane", "Oblivion Wing", "Peck", "Pluck", "Shadow Blast", "Shadow Blitz", "Shadow Bolt", 
        "Shadow Break", "Shadow Chill", "Shadow End", "Shadow Fire", "Shadow Rush", "Sky Attack", "Sky Drop", 
        "Water Pulse", "Wing Attack"]
    userOrAdjacentAlly = ["Acupressure"]
    allAdjacentFoes = ["Acid", "Air Cutter", "Astral Barrage", "Bleakwind Storm", "Blizzard", "Bouncy Bubble", 
        "Breaking Swipe", "Bubble", "Burning Jealousy", "Captivate", "Clanging Scales", "Clangorous Soulblaze", 
        "Core Enforcer", "Cotton Spore", "Dark Void", "Dazzling Gleam", "Diamond Storm", "Disarming Voice", 
        "Dragon Energy", "Electroweb", "Eruption", "Fiery Wrath", "G-Max Snooze", "Glacial Lance", "Glaciate", 
        "Growl", "Heal Block", "Heat Wave", "Hyper Voice", "Icy Wind", "Incinerate", "Land's Wrath", "Leer", 
        "Make It Rain", "Mortal Spin", "Muddy Water", "Origin Pulse", "Overdrive", "Poison Gas", "Powder Snow", 
        "Precipice Blades", "Razor Leaf", "Razor Wind", "Relic Song", "Rock Slide", "Sandsear Storm", "Shell Trap", 
        "Snarl", "Splishy Splash", "Springtide Storm", "String Shot", "Struggle Bug", "Sweet Scent", "Swift", 
        "Tail Whip", "Thousand Arrows", "Thousand Waves", "Twister", "Venom Drench", "Water Spout", "Wildbolt Storm"]
    allAdjacent = ["Boomburst", "Brutal Swing", "Bulldoze", "Corrosive Gas", "Discharge", "Earthquake", "Explosion", 
        "Lava Plume", "Magnitude", "Mind Blown", "Misty Explosion", "Parabolic Charge", "Petal Blizzard", "Searing Shot", 
        "Self-Destruct", "Sludge Wave", "Sparkling Aria", "Surf", "Synchronoise", "Teeter Dance"]
    allAllies = ["Coaching"]
    allFoes = ["Shadow Down", "Shadow Hold", "Shadow Mist", "Shadow Panic", "Shadow Rave", "Shadow Storm", "Shadow Wave", 
        "Spikes", "Stealth Rock", "Sticky Web", "Toxic Spikes"]
    allPokemon = ["Chilly Reception", "Court Change", "Electric Terrain", "Fairy Lock", "Flower Shield", "Grassy Terrain", 
        "Gravity", "Hail", "Haze", "Ion Deluge", "Magic Room", "Misty Terrain", "Mud Sport", "Perish Song", "Psychic Terrain", 
        "Rain Dance", "Rototiller", "Sandstorm", "Shadow Half", "Shadow Shed", "Shadow Sky", "Snowscape", 
        "Sunny Day", "Teatime", "Trick Room", "Water Sport", "Wonder Room"]
    adjacentAlly = ["Aromatic Mist", "Helping Hand", "Hold Hands"]
    user = ["Acid Armor", "Agility", "Ally Switch", "Amnesia", "Aqua Ring", "Assist", "Autotomize", "Baneful Bunker", 
        "Barrier", "Baton Pass", "Belly Drum", "Bide", "Bulk Up", "Calm Mind", "Camouflage", "Celebrate", "Charge", 
        "Clangorous Soul", "Coil", "Comeuppance", "Conversion", "Copycat", "Cosmic Power", "Cotton Guard", "Counter", 
        "Curse", "Defend Order", "Defense Curl", "Destiny Bond", "Detect", "Double Team", "Dragon Dance", "Endure", 
        "Extreme Evoboost", "Fillet Away", "Focus Energy", "Follow Me", "Geomancy", "Growth", "Grudge", "Harden", 
        "Heal Order", "Healing Wish", "Hone Claws", "Imprison", "Ingrain", "Iron Defense", "King's Shield", "Laser Focus", 
        "Lunar Dance", "Magic Coat", "Magnet Rise", "Max Guard", "Meditate", "Metal Burst", "Metronome", "Milk Drink", 
        "Minimize", "Mirror Coat", "Moonlight", "Morning Sun", "Nasty Plot", "No Retreat", "Obstruct", "Outrage", "Petal Dance", 
        "Power Shift", "Power Trick", "Protect", "Quiver Dance", "Rage Powder", "Raging Fury", "Recover", "Recycle", "Refresh", 
        "Rest", "Revival Blessing", "Rock Polish", "Roost", "Sharpen", "Shed Tail", "Shell Smash", "Shelter", "Shift Gear", 
        "Shore Up", "Silk Trap", "Slack Off", "Sleep Talk", "Snatch", "Soft-Boiled", "Spiky Shield", "Splash", "Stockpile", 
        "Struggle", "Stuff Cheeks", "Substitute", "Swallow", "Swords Dance", "Synthesis", "Tail Glow", "Take Heart", "Teleport", 
        "Thrash", "Tidy Up", "Uproar", "Victory Dance", "Wish", "Withdraw", "Work Up"]
    userAndAllies = ["Aromatherapy", "Aurora Veil", "Crafty Shield", "Gear Up", "Happy Hour", "Heal Bell", "Howl", 
        "Jungle Healing", "Life Dew", "Light Screen", "Lucky Chant", "Lunar Blessing", "Magnetic Flux", "Mat Block", 
        "Mist", "Quick Guard", "Reflect", "Safeguard", "Tailwind", "Wide Guard"]

    if(user.count(name) != 0):
        return 1
    if(userOrAdjacentAlly.count(name) != 0):
        return 2
    if(adjacentAlly.count(name) != 0):
        return 3
    if(adjacentFoe.count(name) != 0):
        return 4
    if(anyOther.count(name) != 0):
        return 5
    if(userAndAllies.count(name) != 0):
        return 6
    if(allAllies.count(name) != 0):
        return 7
    if(allFoes.count(name) != 0):
        return 8
    if(allAdjacent.count(name) != 0):
        return 9
    if(allAdjacentFoes.count(name) != 0):
        return 10
    if(allPokemon.count(name) != 0):
        return 11
    return 0

def getRecoilPercent(name):
    fullHP = ["Self-Destruct"]
    halfMaxHP = ["Chloroblast"]
    quarterMaxHP = ["Struggle"]
    halfCurHP = ["Shadow End"]
    halfRecoil = ["Head Smash", "Light of Ruin"]
    thirdRecoil = ["Brave Bird", "Double-Edge", "Flare Blitz", "Volt Tackle", "Wave Crash", "Wood Hammer"]
    quarterRecoil = ["Head Charge", "Submission", "Take Down", "Wild Charge"]

    return 0

def determineFlags(name):
    flags = []
    
    contactMoves = ["Pound", "Karate Chop", "Double Slap", "Comet Punch", "Mega Punch", 
        "Fire Punch", "Ice Punch", "Thunder Punch", "Scratch", "Vise Grip", "Guillotine", 
        "Cut", "Wing Attack", "Fly", "Bind", "Slam", "Vine Whip", "Stomp", "Double Kick", 
        "Mega Kick", "Jump Kick", "Rolling Kick", "Headbutt", "Horn Attack", "Fury Attack", 
        "Horn Drill", "Tackle", "Body Slam", "Wrap", "Take Down", "Thrash", "Double-Edge", 
        "Bite", "Peck", "Drill Peck", "Submission", "Low Kick", "Counter", "Seismic Toss", 
        "Strength", "Petal Dance", "Dig", "Quick Attack", "Rage", "Bide", "Lick", "Waterfall", 
        "Clamp", "Skull Bash", "Constrict", "High Jump Kick", "Leech Life", "Dizzy Punch", 
        "Crabhammer", "Fury Swipes", "Hyper Fang", "Super Fang", "Slash", "Struggle", 
        "Triple Kick", "Thief", "Flame Wheel", "Flail", "Reversal", "Mach Punch", "Feint Attack", 
        "Outrage", "Rollout", "False Swipe", "Spark", "Fury Cutter", "Steel Wing", "Return", 
        "Frustration", "Dynamic Punch", "Megahorn", "Pursuit", "Rapid Spin", "Iron Tail", 
        "Metal Claw", "Vital Throw", "Cross Chop", "Crunch", "Extreme Speed", "Rock Smash", 
        "Fake Out", "Facade", "Focus Punch", "Smelling Salts", "Superpower", "Revenge", 
        "Brick Break", "Knock Off", "Endeavor", "Dive", "Arm Thrust", "Blaze Kick", "Ice Ball", 
        "Needle Arm", "Poison Fang", "Crush Claw", "Meteor Mash", "Astonish", "Shadow Punch", 
        "Sky Uppercut", "Aerial Ace", "Dragon Claw", "Bounce", "Poison Tail", "Covet", 
        "Volt Tackle", "Leaf Blade", "Wake-Up Slap", "Hammer Arm", "Gyro Ball", "Pluck", 
        "U-turn", "Close Combat", "Payback", "Assurance", "Trump Card", "Wring Out", 
        "Punishment", "Last Resort", "Sucker Punch", "Flare Blitz", "Force Palm", "Poison Jab", 
        "Night Slash", "Aqua Tail", "X-Scissor", "Dragon Rush", "Drain Punch", "Brave Bird", 
        "Giga Impact", "Bullet Punch", "Avalanche", "Shadow Claw", "Thunder Fang", "Ice Fang", 
        "Fire Fang", "Shadow Sneak", "Zen Headbutt", "Rock Climb", "Power Whip", "Cross Poison", 
        "Iron Head", "Grass Knot", "Bug Bite", "Wood Hammer", "Aqua Jet", "Head Smash", 
        "Double Hit", "Crush Grip", "Shadow Force", "Storm Throw", "Heavy Slam", "Flame Charge", 
        "Low Sweep", "Foul Play", "Chip Away", "Sky Drop", "Circle Throw", "Acrobatics", "Retaliate", 
        "Dragon Tail", "Wild Charge", "Drill Run", "Dual Chop", "Heart Stamp", "Horn Leech", 
        "Sacred Sword", "Razor Shell", "Heat Crash", "Steamroller", "Tail Slap", "Head Charge", 
        "Gear Grind", "Bolt Strike", "V-create", "Flying Press", "Fell Stinger", "Phantom Force", 
        "Draining Kiss", "Play Rough", "Nuzzle", "Hold Back", "Infestation", "Power-Up Punch", 
        "Dragon Ascent", "Catastropika", "First Impression", "Darkest Lariat", "Ice Hammer", 
        "High Horsepower", "Solar Blade", "Throat Chop", "Anchor Shot", "Lunge", "Fire Lash", 
        "Power Trip", "Smart Strike", "Trop Kick", "Dragon Hammer", "Brutal Swing", "Malicious Moonsault", 
        "Soul-Stealing 7-Star Strike", "Pulverizing Pancake", "Psychic Fangs", "Stomping Tantrum", 
        "Accelerock", "Liquidation", "Spectral Thief", "Sunsteel Strike", "Zing Zap", "Multi-Attack", 
        "Plasma Fists", "Searing Sunraze Smash", "Let's Snuggle Forever", "Zippy Zap", "Floaty Fall", 
        "Sizzly Slide", "Veevee Volley", "Double Iron Bash", "Jaw Lock", "Bolt Beak", "Fishious Rend", 
        "Body Press", "Snap Trap", "Behemoth Blade", "Behemoth Bash", "Breaking Swipe", "Branch Poke", 
        "Spirit Break", "False Surrender", "Steel Roller", "Grassy Glide", "Skitter Smack", "Lash Out", 
        "Flip Turn", "Triple Axel", "Dual Wingbeat", "Wicked Blow", "Surging Strikes", "Thunderous Kick", 
        "Dire Claw", "Psyshield Bash", "Stone Axe", "Wave Crash", "Headlong Rush", "Ceaseless Edge", 
        "Axe Kick", "Jet Punch", "Spin Out", "Population Bomb", "Ice Spinner", "Glaive Rush", "Triple Dive", 
        "Mortal Spin", "Kowtow Cleave", "Aqua Step", "Raging Bull", "Collision Course", "Electro Drift", 
        "Pounce", "Trailblaze", "Hyper Drill", "Rage Fist", "Bitter Blade", "Double Shock", "Comeuppance"]
    print("Contact Moves: " + str(len(contactMoves)))
    if(contactMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    soundMoves = ["Boomburst", "Bug Buzz", "Chatter", "Clanging Scales", "Clangorous Soul", 
        "Clangorous Soulblaze", "Confide", "Disarming Voice", "Echoed Voice", "Eerie Spell", 
        "Grass Whistle", "Growl", "Heal Bell", "Howl", "Hyper Voice", "Metal Sound", "Noble Roar", 
        "Overdrive", "Parting Shot", "Perish Song", "Relic Song", "Roar", "Round", "Screech", 
        "Shadow Panic", "Sing", "Snarl", "Snore", "Sparkling Aria", "Supersonic", "Torch Song", "Uproar"]
    print("Sound Moves: " + str(len(soundMoves)))
    if(soundMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    punchMoves = ["Bullet Punch", "Comet Punch", "Dizzy Punch", "Double Iron Bash", "Drain Punch", 
        "Dynamic Punch", "Fire Punch", "Focus Punch", "Hammer Arm", "Headlong Rush", "Ice Hammer", 
        "Ice Punch", "Jet Punch", "Mach Punch", "Mega Punch", "Meteor Mash", "Plasma Fists", "Power-Up Punch", 
        "Rage Fist", "Shadow Punch", "Sky Uppercut", "Surging Strikes", "Thunder Punch", "Wicked Blow"]
    print("Punch Moves: " + str(len(punchMoves)))
    if(punchMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    biteMoves = ["Bite", "Crunch", "Fire Fang", "Fishious Rend", "Hyper Fang", "Ice Fang", 
        "Jaw Lock", "Poison Fang", "Psychic Fangs", "Thunder Fang"]
    print("Bite Moves: " + str(len(biteMoves)))
    if(biteMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    snatchableMoves = ["Acid Armor", "Agility", "Amnesia", "Aqua Ring", "Aromatherapy", 
        "Aurora Veil", "Autotomize", "Barrier", "Belly Drum", "Bulk Up", "Calm Mind", 
        "Camouflage", "Charge", "Coil", "Conversion", "Cosmic Power", "Cotton Guard", 
        "Defend Order", "Defense Curl", "Double Team", "Dragon Dance", "Focus Energy", 
        "Gear Up", "Growth", "Harden", "Heal Bell", "Heal Order", "Healing Wish", 
        "Hone Claws", "Howl", "Imprison", "Ingrain", "Iron Defense", "Laser Focus", 
        "Light Screen", "Lucky Chant", "Lunar Dance", "Magnet Rise", "Magnetic Flux", 
        "Mat Block", "Meditate", "Milk Drink", "Minimize", "Mist", "Moonlight", 
        "Morning Sun", "Nasty Plot", "Power Trick", "Quick Guard", "Quiver Dance", 
        "Recover", "Recycle", "Reflect", "Refresh", "Rest", "Rock Polish", "Roost", 
        "Safeguard", "Sharpen", "Shell Smash", "Shift Gear", "Shore Up", "Slack Off", 
        "Soft-Boiled", "Stockpile", "Substitute", "Swallow", "Swords Dance", "Synthesis", 
        "Tail Glow", "Tailwind", "Wide Guard", "Wish", "Withdraw", "Work Up"]
    print("Snatch Moves: " + str(len(snatchableMoves)))
    if(snatchableMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    sliceMoves = ["Aerial Ace", "Air Cutter", "Air Slash", "Aqua Cutter", "Behemoth Blade", 
        "Bitter Blade", "Ceaseless Edge", "Cross Poison", "Cut", "Fury Cutter", "Kowtow Cleave", 
        "Leaf Blade", "Night Slash", "Population Bomb", "Psyblade", "Psycho Cut", "Razor Leaf", 
        "Razor Shell", "Sacred Sword", "Slash", "Solar Blade", "Stone Axe", "X-Scissor"]
    print("Slice Moves: " + str(len(sliceMoves)))
    if(sliceMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    bulletMoves = ["Bullet Seed", "Egg Bomb", "Gyro Ball", "Ice Ball", "Magnet Bomb", "Pyro Ball", 
        "Rock Blast", "Rock Wrecker", "Seed Bomb", "Acid Spray", "Aura Sphere", "Electro Ball", 
        "Energy Ball", "Focus Blast", "Mist Ball", "Mud Bomb", "Octazooka", "Pollen Puff", 
        "Searing Shot", "Shadow Ball", "Sludge Bomb", "Weather Ball", "Zap Cannon"]
    print("Bullet Moves: " + str(len(bulletMoves)))
    if(bulletMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    windMoves = ["Air Cutter", "Bleakwind Storm", "Blizzard", "Fairy Wind", "Gust", "Heat Wave", 
        "Hurricane", "Icy Wind", "Petal Blizzard", "Sandsear Storm", "Sandstorm", "Springtide Storm", 
        "Tailwind", "Twister", "Whirlwind", "Wildbolt Storm"]
    print("Wind Moves: " + str(len(windMoves)))
    if(windMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    powderMoves = ["Cotton Spore", "Magic Powder", "Poison Powder", "Powder", "Rage Powder", 
        "Sleep Powder", "Spore", "Stun Spore"]
    print("Powder Moves: " + str(len(powderMoves)))
    if(powderMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)

    nonMetronomeMoves = ["After You", "Apple Acid", "Armor Cannon", "Assist", "Astral Barrage", 
        "Aura Wheel", "Baneful Bunker", "Beak Blast", "Behemoth Bash", "Behemoth Blade", 
        "Belch", "Bestow", "Blazing Torque", "Body Press", "Branch Poke", "Breaking Swipe", 
        "Celebrate", "Chatter", "Chilling Water", "Chilly Reception", "Clangorous Soul", 
        "Collision Course", "Combat Torque", "Comeuppance", "Copycat", "Counter", "Covet", 
        "Crafty Shield", "Decorate", "Destiny Bond", "Detect", "Diamond Storm", "Doodle", 
        "Double Iron Bash", "Double Shock", "Dragon Ascent", "Dragon Energy", "Drum Beating", 
        "Dynamax Cannon", "Electro Drift", "Endure", "Eternabeam", "False Surrender", "Feint", 
        "Fiery Wrath", "Fillet Away", "Fleur Cannon", "Focus Punch", "Follow Me", "Freeze Shock", 
        "Freezing Glare", "Glacial Lance", "Grav Apple", "Helping Hand", "Hold Hands", "Hyper Drill", 
        "Hyperspace Fury", "Hyperspace Hole", "Ice Burn", "Instruct", "Jet Punch", "Jungle Healing", 
        "King's Shield", "Life Dew", "Light of Ruin", "Magical Torque", "Make It Rain", "Mat Block", 
        "Me First", "Meteor Assault", "Metronome", "Mimic", "Mind Blown", "Mirror Coat", "Mirror Move", 
        "Moongeist Beam", "Nature Power", "Nature's Madness", "Noxious Torque", "Obstruct", "Order Up", 
        "Origin Pulse", "Overdrive", "Photon Geyser", "Plasma Fists", "Population Bomb", "Pounce", 
        "Power Shift", "Precipice Blades", "Protect", "Pyro Ball", "Quash", "Quick Guard", "Rage Fist", 
        "Rage Powder", "Raging Bull", "Raging Fury", "Relic Song", "Revival Blessing", "Ruination", 
        "Salt Cure", "Secret Sword", "Shed Tail", "Shell Trap", "Silk Trap", "Sketch", "Sleep Talk", 
        "Snap Trap", "Snarl", "Snatch", "Snore", "Snowscape", "Spectral Thief", "Spicy Extract", 
        "Spiky Shield", "Spirit Break", "Spotlight", "Springtide Storm", "Steam Eruption", "Steel Beam", 
        "Strange Steam", "Struggle", "Sunsteel Strike", "Surging Strikes", "Switcheroo", "Techno Blast", 
        "Thief", "Thousand Arrows", "Thousand Waves", "Thunder Cage", "Thunderous Kick", "Tidy Up", 
        "Trailblaze", "Transform", "Trick", "Twin Beam", "V-create", "Wicked Blow", "Wicked Torque", "Wide Guard",
        "Shadow Blitz", "Shadow Rush", "Shadow Break", "Shadow End", "Shadow Wave", "Shadow Rave", 
        "Shadow Storm", "Shadow Fire", "Shadow Bolt", "Shadow Chill", "Shadow Blast", "Shadow Sky", 
        "Shadow Hold", "Shadow Mist", "Shadow Panic", "Shadow Down", "Shadow Shed", "Shadow Half", 
        "Max Airstream", "Max Darkness", "Max Flare", "Max Flutterby", "Max Geyser", "Max Hailstorm", 
        "Max Knuckle", "Max Lightning", "Max Mindstorm", "Max Ooze", "Max Overgrowth", "Max Phantasm", 
        "Max Quake", "Max Rockfall", "Max Starfall", "Max Steelspike", "Max Strike", "Max Wyrmwind", "Max Guard",
        "G-Max Befuddle", "G-Max Cannonade", "G-Max Centiferno", "G-Max Chi Strike", "G-Max Cuddle", 
        "G-Max Depletion", "G-Max Finale", "G-Max Foam Burst", "G-Max Gold Rush", "G-Max Gravitas", 
        "G-Max Malodor", "G-Max Meltdown", "G-Max One Blow", "G-Max Rapid Flow", "G-Max Replenish", 
        "G-Max Resonance", "G-Max Sandblast", "G-Max Smite", "G-Max Snooze", "G-Max Steelsurge", 
        "G-Max Stonesurge", "G-Max Stun Shock", "G-Max Sweetness", "G-Max Tartness", "G-Max Terror", 
        "G-Max Vine Lash", "G-Max Volcalith", "G-Max Volt Crash", "G-Max Wildfire", "G-Max Wind Rage", 
        "G-Max Drum Solo", "G-Max Fireball", "G-Max Hydrosnipe", "10,000,000 Volt Thunderbolt", 
        "Acid Downpour", "All-Out Pummeling", "Black Hole Eclipse", "Bloom Doom", "Breakneck Blitz", 
        "Catastropika", "Clangorous Soulblaze", "Continental Crush", "Corkscrew Crash", "Devastating Drake", 
        "Extreme Evoboost", "Genesis Supernova", "Gigavolt Havoc", "Guardian of Alola", "Hydro Vortex", 
        "Inferno Overdrive", "Let's Snuggle Forever", "Light That Burns the Sky", "Malicious Moonsault", 
        "Menacing Moonraze Maelstrom", "Never-Ending Nightmare", "Oceanic Operetta", "Pulverizing Pancake", 
        "Savage Spin-Out", "Searing Sunraze Smash", "Shattered Psyche", "Sinister Arrow Raid", 
        "Soul-Stealing 7-Star Strike", "Splintered Stormshards", "Stoked Sparksurfer", "Subzero Slammer", 
        "Supersonic Skystrike", "Tectonic Rage", "Twinkle Tackle"]
    print("Non-Metronome Moves: " + str(len(nonMetronomeMoves)))
    if(nonMetronomeMoves.count(name) != 0):
        flags.append(0)
    else:
        flags.append(1)

    gravityMoves = ["Bounce", "Fly", "Flying Press", "High Jump Kick", "Jump Kick", "Magnet Rise", 
        "Sky Drop", "Splash", "Telekinesis"]
    print("Gravity Moves: " + str(len(gravityMoves)))
    if(gravityMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0)   

    #All fire moves (except hidden power) + scald can thaw a frozen target 
    defrostingMoves = ["Burn Up", "Flame Wheel", "Flare Blitz", "Fusion Flare", "Pyro Ball", 
        "Sacred Fire", "Scald", "Scorching Sands", "Steam Eruption"]
    print("Defrost Moves: " + str(len(defrostingMoves)))
    if(defrostingMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0) 

    #Reflected by Magic Coat and Magic Bounce
    reflectedMoves = ["Attract", "Baby-Doll Eyes", "Block", "Captivate", "Charm", "Confide", 
        "Confuse Ray", "Corrosive Gas", "Cotton Spore", "Dark Void", "Defog", "Disable", 
        "Eerie Impulse", "Embargo", "Encore", "Entrainment", "Fake Tears", "Feather Dance", 
        "Flash", "Flatter", "Floral Healing", "Foresight", "Forest's Curse", "Gastro Acid", 
        "Glare", "Grass Whistle", "Growl", "Heal Block", "Heal Pulse", "Hypnosis", "Kinesis", 
        "Leech Seed", "Leer", "Lovely Kiss", "Magic Powder", "Mean Look", "Metal Sound", 
        "Miracle Eye", "Noble Roar", "Odor Sleuth", "Parting Shot", "Play Nice", "Poison Gas", 
        "Poison Powder", "Powder", "Purify", "Roar", "Sand Attack", "Scary Face", "Screech", 
        "Simple Beam", "Sing", "Sleep Powder", "Smokescreen", "Soak", "Spider Web", "Spikes", 
        "Spite", "Spore", "Spotlight", "Stealth Rock", "Sticky Web", "Strength Sap", "String Shot", 
        "Stun Spore", "Supersonic", "Swagger", "Sweet Kiss", "Sweet Scent", "Tail Whip", "Tar Shot", 
        "Taunt", "Tearful Look", "Telekinesis", "Thunder Wave", "Tickle", "Topsy-Turvy", "Torment", 
        "Toxic", "Toxic Spikes", "Toxic Thread", "Trick-or-Treat", "Venom Drench", "Whirlwind", 
        "Will-O-Wisp", "Worry Seed", "Yawn"]
    print("Reflected Moves: " + str(len(reflectedMoves)))
    if(reflectedMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0) 

    #TODO: Update protected list (status moves are incomplete)

    notProtectedMoves = ["Max Airstream", "Max Darkness", "Max Flare", "Max Flutterby", "Max Geyser", 
        "Max Hailstorm", "Max Knuckle", "Max Lightning", "Max Mindstorm", "Max Ooze", "Max Overgrowth", 
        "Max Phantasm", "Max Quake", "Max Rockfall", "Max Starfall", "Max Steelspike", "Max Strike", 
        "Max Wyrmwind", "10,000,000 Volt Thunderbolt", "Acid Downpour", "All-Out Pummeling", 
        "Black Hole Eclipse", "Bloom Doom", "Breakneck Blitz", "Catastropika", "Clangorous Soulblaze", 
        "Continental Crush", "Corkscrew Crash", "Devastating Drake", "Genesis Supernova", "Gigavolt Havoc", 
        "Guardian of Alola", "Hydro Vortex", "Inferno Overdrive", "Let's Snuggle Forever", 
        "Light That Burns the Sky", "Malicious Moonsault", "Menacing Moonraze Maelstrom", "Never-Ending Nightmare", 
        "Oceanic Operetta", "Pulverizing Pancake", "Savage Spin-Out", "Searing Sunraze Smash", "Shattered Psyche", 
        "Sinister Arrow Raid", "Soul-Stealing 7-Star Strike", "Splintered Stormshards", "Stoked Sparksurfer", 
        "Subzero Slammer", "Supersonic Skystrike", "Tectonic Rage", "Twinkle Tackle", "G-Max Befuddle", 
        "G-Max Cannonade", "G-Max Centiferno", "G-Max Chi Strike", "G-Max Cuddle", "G-Max Depletion", 
        "G-Max Drum Solo", "G-Max Finale", "G-Max Fireball", "G-Max Foam Burst", "G-Max Gold Rush", 
        "G-Max Gravitas", "G-Max Hydrosnipe", "G-Max Malodor", "G-Max Meltdown", "G-Max One Blow", 
        "G-Max Rapid Flow", "G-Max Replenish", "G-Max Resonance", "G-Max Sandblast", "G-Max Smite", 
        "G-Max Snooze", "G-Max Steelsurge", "G-Max Stonesurge", "G-Max Stun Shock", "G-Max Sweetness", 
        "G-Max Tartness", "G-Max Terror", "G-Max Vine Lash", "G-Max Volcalith", "G-Max Volt Crash", 
        "G-Max Wildfire", "G-Max Wind Rage", "Feint", "Hyperspace Fury", "Hyperspace Hole", "Phantom Force", 
        "Shadow Force", "Hyper Drill", "Imprison", "Perish Song", "Psych Up", "Doom Desire", "Future Sight", "Transform"]
    print("Not Protected Moves: " + str(len(notProtectedMoves)))
    if(notProtectedMoves.count(name) != 0):
        flags.append(0)
    else:
        flags.append(1) 

    #Copyable by Mirror Move (May not implement mirror move)
    copyableMoves = []
    if(copyableMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0) 

    #Fly, Bounce, Sky Drop
    hitsAirMoves = ["Gust", "Thunder", "Twister", "Sky Uppercut", "Hurricane", "Smack Down", "Thousand Arrows"]
    print("Hits Fly: " + str(len(hitsAirMoves)))
    if(hitsAirMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0) 

    hitsDigMoves = ["Helping Hand", "Toxic", "Earthquake", "Magnitude", "Fissure"]
    print("Hits Dig: " + str(len(hitsDigMoves)))
    if(hitsDigMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0) 

    hitsDiveMoves = ["Surf", "Whirlpool"]
    print("Hits Dive: " + str(len(hitsDiveMoves)))
    if(hitsDiveMoves.count(name) != 0):
        flags.append(1)
    else:
        flags.append(0) 

    return flags

with open('Moves.csv', 'r') as input:
    with open('Output.csv', 'w') as output:
        for line in input:
            fields = line.split('|')
            name = fields[1]
            moveType = getTypeID(fields[2])
            moveCat = getCatID(fields[3])
            pp = fields[4]
            power = cleanPower(name, fields[5])
            accuracy = cleanAccuracy(fields[6])
            priority = getPriority(name)
            critRate = getCritRate(name, moveCat)
            moveRange = getRange(name)
            recoilPercent = getRecoilPercent(name)

            flags = determineFlags(name)
            if([0, 4, 5, 8, 9, 10].count(moveRange) == 0):
                flags[13] = 0
            if(["Thrash", "Outrage", "Struggle", "Petal Dance", "Uproar", "Raging Fury"].count(name) != 0):
                flags[13] = 1
            output.write(name + '|' + str(flags[17]) + '\n')
            #output.write(name + '|' + str(moveType) + '|' + str(moveCat) + '|' + pp + '|' + power + '|' + str(priority) + '\n') #Write out all relevant data that will be used to create the Unity Asset


#Status: Every current field of PokemonMoveBase is determined except for the mirror move field due to lack of data
#The protected field also needs to be updated as it is not 100% accurate (Will probably need more time and a deeper dive)
#Will want to add many more flags and variables for the various categories listed on Bulbapedia: https://bulbapedia.bulbagarden.net/wiki/Category:Moves_by_effect