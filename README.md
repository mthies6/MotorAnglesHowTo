The Unity project and scripts within it are used to gather the angles of motors form a digital twin of a real life robotic. 
Used in the Electronic Artrium Mr. Bee's Journey 

# Steps for Setup
1. Create Blender model with armature. 
    1. Suggested approach: multiple separate objects joined by the armature
    2. In object mode, shift + click all the mesh objects, then the armature last. Parent the armature to the mesh with Command P → Armature Deform → With Automatic Weights 
2. In pose mode, position the model in its desired default position by moving the bones of the armature.
3. In object mode, apply all transforms (press A to select all and then shift + A → all transforms). This ensures that the current position is set as the model’s default in both Blender and Unity.
4. Make an animation clip by altering the armature in pose mode (can use NLA editor too, but requires different export settings)
    1. Suggestion: Use auto-keying in the animation timeline view
5. Export the model as an FBX
    1. Note: Unity and Blender use different default axes so we must account for that 
    2. Export settings: File → Export → FBX
        1. Include → 
            1. Limit to: Selected Objects
            2. Select Object Types Armature and Mesh only 
        2. Transform →
            1. Apply Scalings: FBX All
            2. Forward: -Y Forward 
            3. Only check Apply Unit
        3. Geometry → Apply Modifiers 
        4. Armature
            1. Check Add Leaf Bones
        5. Check Bake Animation
6. Open the FBX model in the Unity project assets.
    1. Select the model in the file viewer and select Rig in the Inspector. 
    2. Set Animation Type to Generic and Avatar Definition to Create From This Model. Then click Apply
7. Place the model into the scene
    1. Give it the AnimationValues script component
        1. Note the empty Anim CIip and Rotating Object parameters. These will be filled in during later steps.
    2. The model should already have an Animator component because of the avatar created in the last step 
        1. Create an AnimationController for the model and drag it into the controller slot of the Animator component (Assets → Create → Animator Controller)
8. Expand the model within your file viewer using the arrow on the right side of the.
    1. Select the animation clip (triangle with lines trailing to the left) and duplicate it outside of the model. This allows the clip to be edited (instead of read only)
    2. Drag and drop the new copy of the clip into the models inspector under Animation Values → Anim Clip
9. Now, double click your Animator Controller, which will take you to the Animator window (*not* the Animation window)
    1. Right click → Create State → Empty 
    2. This should create a rectangle that has an arrow pointing to it from the “Entry” rectangle.
    3. Select this new state you created, and look at the inspector. Drag and drop the same animation clip from the last step into Motion
10. Double click the animation clip (the same duplicate) in the file viewer. This will open the Animation Unity window in the Dopesheet view.
    1. From here you can see what objects are actually moving (rotating). This is indicated by the row in which the keyframe icons exist. 
    2. With the original model selected in the hierarchy, you can double click each row (or bone) and it will highlight the associated bone in the hierarchy.
11. Select these important bones, indicated by the Animation keyframes, from the scene hierarchy and drag them into the main model’s script component Animation Values → Objs
    1. Click the drop down arrow on the left of Objs, the + button for how many bones are highlighted in the animation, then drag the GameObjects into the created open slots
12. Back in the Animation window Dopesheet, add Animation events for each keyframe. The icon is a skinny rectangle with a pointed edge and a plus sign underneath the frame counter.
    1. Each AnimationEvent should call the function AnimationValues → Methods → LogFrame(). You can set this from the inspector when the Event is selected. 
        1. If the function does not show up, you have not correctly attached the AnimationValues script to the model in the scene hierarchy. 
    2. Since animation keyframes can get messy when exporting from Blender and importing to Unity, you can also make any edits to the keyframes here. I suggest doing this ONLY if you have experience with the Unity Animation editor, as it is very finicky.
13. Run the Unity project. Once the animation completes, press space, and the values will be logged to a file within Assets/TextFiles (as well as interpolated values)