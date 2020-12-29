# XorBrute

C# program that bruteforces all [a-Z] or [a-Z 0-9] UTF-8 keys and uses them as XOR keys against XOR encrypted texts (only works for encrypted a-Z messages)

# Example

Example text: `2D4F18031B0D0003154C1A1F190E004C1B09141B4C09010F1E161C180A084C180518074C34203E4C1A1F05010B4C0E4C1C1D09181B154C0B0D01014C0F0003004F070916`

Example input:
```
Preparing the wordlist...
Done!
Enter encrypted text (HEX), or a path starting with '=' to load the file from:24
0A0059034F1B5A1E0308144C3B045C1F4F05464C0E4C46191F09474C1B0946184F01501F1C0D5209
4F185A4C1B0946184F0E47191B091503014D15240E1A504C09195B4D
Select mode:
1. A-z
2. A-z 0-9
>2
Enter the maximal length of the key
>4
Enter the A-z threshold for key testing (Around 85 is fine. Usually >150 confide
nce is for the real key)
>90
Do you want us to use wordlist to increase accuracy of key guesses? It is slower
, but not using it may lead to incorrect, but similar keys. (Y/n)
>Y
Enter amount of threads to use (4 works fine for dual-cored CPUs, 16 for high-en
d PCs)
>4
```

Example output:
```
Bruteforce is done. Combining entries.
Done! Found 4091 entries!
Sorting...
Done!
Time taken: 36,8581269 seconds.
- TOP 20 Best matches -
[1] lol5 : Confidence: 225
[2] qolf : Confidence: 156,159420289855
[3] yolf : Confidence: 150,36231884058
[4] lol2 : Confidence: 147,101449275362
[5] lolf : Confidence: 139,855072463768
[6] lylf : Confidence: 132,608695652174
[7] lllf : Confidence: 132,608695652174
[8] loff : Confidence: 131,159420289855
[9] lhff : Confidence: 131,159420289855
[10] qoof : Confidence: 129,710144927536
[11] qomf : Confidence: 129,710144927536
[12] qokf : Confidence: 129,710144927536
[13] qojf : Confidence: 129,710144927536
[14] lvlf : Confidence: 129,710144927536
[15] lslf : Confidence: 129,710144927536
[16] lnyf : Confidence: 129,710144927536
[17] qoOf : Confidence: 128,260869565217
[18] qoLf : Confidence: 128,260869565217
[19] qoKf : Confidence: 128,260869565217
[20] qoJf : Confidence: 128,260869565217
------------
Enter ID to reveal the decryption or enter 'OUT' to save to output.txt:
```

Real key: `lol5`

Message decryption: 
```
------------
KEY: lol5
Confidence: 225
Result: Hello world! This is a super test message to test brute on! Have fun!
------------
```

# Tips

Try to use a text of no less than 20 characters, but no more than 250. Bigger texts allow for bigger accuracy, but will take more time. The program is useless for texts with XOR keys being hashed, or longer than 6 characters (at least in its current state). The program only works for English texts.