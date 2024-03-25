Dire Dungeons


Oh I just realized I have some videos on YouTube of some of the progress and what I’m going for.  That might help visualize things that I explain below.

https://youtu.be/X08DXMHeon8?si=51yUzeKaQv8_sSco&t=88

There’s a lot of placeholder art in there.  I usually like to get things working first, then make them look pretty. :p  This project is a lot more complicated than the one I’m including, but it’s using a lot of third party assets, so didn’t want to include them.

Most everything inside _bootstrap and _Game I created, aside from the 3d art and animations.

Interesting things to note, would be inside _bootstrap and _bootstrap/ZagaCore.  I created several services as well as installers for these services.  I had the thought that after the initial creation of the service, I no longer needed the ‘installer’ to hang around, so created a way for it to quickly load with a dependency injection using RSG Promises (I come from a JS background so like these).  You can see the list by going to Assets/_bootstrap/Prefabs/RootContext prefab.

So it loads everything, then deletes the installers after the references to the different services and systems have been added to a referencing system called “Refs”.  With this I can bind and unbind things that can be grabbed by other classes.  It can also wait for that thing to be available, and then use it once it is.

I used a linked list for this system, mainly just as a personal challenge, found in the EventService folder.

Hmm, there’s also an audio service, authentication service, Camera, Input, etc etc.  I basically created a Unity package, that I can export and import into a new project and it has everything it needs for a new game.

For this particular game, you need to be logged into Steam, oh I guess you also need to have access, which I haven’t set up any way to give it to people yet.  Maybe I’ll remove that part of the demo, so you can just press Play.

Anyway, the other alternative is to export to APK, and play on Android.  I created this really cool thumb control system where you can drag anywhere on the screen, and it reveals the thumb controller.  This will drag with you if you go past the border, which I really like.  You can also start to drag on the other side and it will instantly jump to the new finger.

To attack, you just tap anywhere with a finger.

I am creating my own Timeline tracks to handle the combat.  If you look at _Game/Prefabs there are some prefabs called GreatAxe_Auto_1, GreatAxe_Auto_2, GreatAxe_Auto_3.  As you spam tap the screen, it will automatically go into these 3 attacks.  I was also working on a custom weapon trail, because none of the others I saw on the internet was cool.  This is also controlled by the Timeline.

What is really cool, is the weapon and armor system.  When the game first starts, it has an empty rigged skeleton, and that is all(actually I think in this version I hard attached the axe to work on the weapon trails).  Then I dynamically load the pieces of armor and weapons through an attachment system I created.  These are found in _Game/AttachPoints and used in the TransformMap.  The mesh is then generated on the skeleton and you can see the character.
This project was gutted from a previous version where it had multiplayer support using the new unity multiplayer relay and lobby stuff, but I wanted to work on the actions, attacks, and abilities in this one first, before trying to express them across the network.

When you first use an attack, it creates these control objects which get recycled and enabled/disabled by the ActionControl.  I ran into an issue where I wanted to play multiple Timelines at the same time, so needed a game object to control these.

Another interesting idea I am fleshing out this ‘brain’ vs ‘data’ concept.  In this project it’s found in the “SwarmControl”.  The idea is to have data objects with references to components, and then the “brain” implements actions for these data objects.  It’s kinda like ECS, where the System and Control are the brain, and the Entity is the data object.

I’m sure there's a lot more I’m missing, but I think these are the coolest parts of the project.  Oh it also uses the new InputSystem for input.  For the PC version, it uses WASD and Left Mouse.

My plan for this is to make it like an intellisense type thing where it will automatically know what to do based on inference and what you are looking at.  For example, if you’re looking at a tree, and tap, it will know to cut the tree.  Or if you’re looking at a door and tap, it will know to open the door.  If you’re facing enemies, it will know to attack.

I think the biggest challenges to overcome, was using a linked list for the event system.  In retrospect I spent way too much time on that, and had to start over a couple times.  The other thing is the ability systems.  I want it all to be dynamic, based on the weapon you chose, so there will be a lot of streaming things in and assembling it altogether.  The third thing are shaders.  I have a certain look and feel I’m going for, and won’t settle for less than that vision.


The dungeon generator is using a plugin called Dungeon Architect, but that was probably another challenge, getting the inside and outside walls to properly generate corners of every combination.  It uses a pattern matching system, which I’ll probably end up borrowing and make my own system.
