# XorBrute

C# program that bruteforces all [a-Z] or [a-Z 0-9] UTF-8 keys and uses them as XOR keys against XOR encrypted texts (only works for encrypted a-Z messages)

# Example

Example text: `2D4F18031B0D0003154C1A1F190E004C1B09141B4C09010F1E161C180A084C180518074C34203E4C1A1F05010B4C0E4C1C1D09181B154C0B0D01014C0F0003004F070916`

Example input:
```
Preparing the wordlist...
Done!
Enter encrypted text (HEX), or a path starting with '=' to load the file from:2D
4F18031B0D0003154C1A1F190E004C1B09141B4C09010F1E161C180A084C180518074C34203E4C1A
1F05010B4C0E4C1C1D09181B154C0B0D01014C0F0003004F070916
Select mode:
1. A-z
2. A-z 0-9
>1
Enter the maximal length of the key:
4
Enter the A-z threshold for key testing (in percents. Around 0,75 is fine):
0,9
```

Example output:
```
Bruteforce is done. Found 3199 good entries!
Filtering started...
Filtering is done!
Sorting...
Done!
Time taken: 20,7882216 seconds.
- TOP 20 Best matches -
[1] lol : Confidence: 2,4
[2] lol : Confidence: 2,4
[3] LoMn : Confidence: 1,15
[4] LoMl : Confidence: 1,15
[5] LoLn : Confidence: 1,15
[6] LoLl : Confidence: 1,15
[7] Loss : Confidence: 1,15
[8] Losn : Confidence: 1,15
[9] Losm : Confidence: 1,15
[10] Losl : Confidence: 1,15
[11] Lons : Confidence: 1,15
[12] Lonn : Confidence: 1,15
[13] Lonm : Confidence: 1,15
[14] Lonl : Confidence: 1,15
[15] Loms : Confidence: 1,15
[16] Lomn : Confidence: 1,15
[17] Lomm : Confidence: 1,15
[18] Loml : Confidence: 1,15
[19] Lomk : Confidence: 1,15
[20] Lols : Confidence: 1,15
------------
```

Real key: `lol`

Message decryption: ```
------------
KEY: lol
Confidence: 2,4
Result: A totally usual text encrypted with XOR using a pretty damn cool key
------------```

# Tips

Try to use a text of no less than 20 characters, but no more than 250. Bigger texts allow for bigger accuracy, but will take more time. The program is useless for texts with XOR keys being hashed, or longer than 6 characters (at least in its current state). The program only works for English texts.