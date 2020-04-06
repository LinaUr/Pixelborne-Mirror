# Pixelborne

This game has been programmed in the seminar Game Programming from the computer graphics research group from the HPI.
Our supervisor was [Willy Scheibel](https://github.com/scheibel). 


This PC-game is a 2D souls-like side scroller with a multiplayer and singleplayer mode. 
The singleplayer involves a story.

The fighting system is inspired by [Nidhogg](http://nidhogggame.com/).

## Games of Life
The seminar was about programming a game and integrating a critical component into the game.
A critical component is something that does something morally wrong by abusing technology.
This reaches from spying on the user to discrimination of any kind.
This component should somehow present the user what it is actually doing.
Thus the user will hopefully learn something about technology and what it is actually capable of.

We decided to spy on the user in various ways and showing the player what we found in a subtle way:
- We are searching the file system for mp3 files and randomly play one of them silently 
over the game music for a certain amount of time.
- We are looking for pictures in the pictures folder of the user and blend them over 
the existing images from the game.
- We are taking a photo with the webcam and record 10 seconds of audio
when the player accidentally hits two keys without other game functionality.


## Scripts
The Scripts can be found in Assets/Scripts.


## Requirements and Setup
The Game is made with [Unity](https://unity3d.com/get-unity/download/archive) 2018.4.12f1.

We are using a VS-Plugin [NAudio](https://github.com/naudio/NAudio) in order to read mp3 files.
The Plugin should be in the plugin folder and thus should not need further setup.

## Links to 3rd-Party Assets and Scripts that we use
Throughout the projects we are using some 3rd party scripts and free assets. 
We manipulated the assets to our needs.

##### Scripts
- [SawWav](http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734)
for converting a unity audio clip to wav and save it to the disk.
- We used small code snippets from other sources as well. Please read the docs.

##### Assets
- [Background / Stage](https://assetstore.unity.com/packages/2d/environments/pixel-dark-forest-136825)
- [Enemies](https://assetstore.unity.com/packages/2d/characters/hero-nad-opponents-animation-140776)
- [Dark King](https://assetstore.unity.com/packages/2d/characters/bandits-pixel-art-104130)
- [Player](https://ramirov.itch.io/vai-drogul)
- [crystal](https://assetstore.unity.com/packages/2d/gui/icons/crystals-collection-42748)

## Known Issues

- possible sharing violation on audioFilePath.txt
- possible sharing violation on ImportantDocuments.txt