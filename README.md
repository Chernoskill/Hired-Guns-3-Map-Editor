# Hired-Guns-3-Map-Editor
Hired Guns Map Editor for Windows
Robert Wilmesmeier, 2022

The program requires Microsoft NET 4.0.

HOTKEYS

Delete:                 Clear selected cells
End:                    Reset cell controls
Page up:                Z level +
Page down:              Z level -
Add/Plus:               Zoom map view in
Subtract/Minus:         Zoom map view out
A:                      Toggle active mode
M:                      Mode: Select
E:                      Mode: Erase
B:                      Mode: Paint
G:                      Mode: Fill
Middle mouse button:    Pick cell

GENERAL

Always back up your original game maps and the Locations.dat file (in the World directory)) before editing or replacing them!

Hover over a control to read it's tooltip.

BASIC WORKFLOW

Always start from cleared cell controls. You can quickly clear the cell controls by either picking an empty cell with the middle mouse button, or by pressing the End key.
While in paint mode, set the cell properties you want, or pick a cell that already has these. Now you can simply click or hold the left mouse button to paint cells, and clear them with the right mouse button. Select an editing filter for things like floor, sky or water painting. Hold the shift key and left mouse button to quickly select multiple cells without having to switch to the select mode. Switch active mode on when you want to directly set cell properties of selected cells, but be careful and remember to switch it off again after you are done. Use the mouse wheel to move between the Z (height) levels, or "slices", of the map.

PLAYING YOUR MAP

Since you need to replace existing maps in order to play them, make sure to backup your original maps. If you want to test your custom map, you could replace one of the quick action or tutorial maps, or 07.map, which is the Abandoned Depot and the first map that is accessible on the world map when starting the campaign. Remember to set the File number property for your map to correspond to the file name, the world map position if it's a campaign map, and to create the Locations.dat file if you changed monster types, worldmap position, map graphic style, or (world map / detailed) descriptions. Read the paragraph "FINISHING A MAP" further dowen below for more info.

CONTROLS

The buttons on the left set the eding mode. The editing mode you will likely use the most is paint. In the center is the map view, showing the map at the current Z level. On the right are the cell controls, allowing you to set cell properties of selected cells if active mode is on, see below or the cell properties for cells you wish to paint or fill afterwards. Different tabs let you view and set different cell property groups, such as doors and buttons. The last tab is for general map properties, such as the worldmap description, game type, and sky variant.

Select: Drag a selection box around cells. The cell properties on the right will be set to those of the selected cells.

Paint: When clicking on a cell with the left mouse button, this cell will be filled with the selected cell properties. Use the filter drop down list on the top bar if you only want to apply certain properties. The middle mouse button will pick the cell under the mouse cursor: the cell controls on the right will be set the this cell's properties. The right button will clear a cell (The filter function can be used while clearing as well). Hold the shift key to drag a selection box.

Fill: Click inside the selected area to set all contained cells to the chosen cell properties.

Erase: Click inside the selected area to clear all contained cells.

Select target in / out / teleport target: While having a button cell selected which has a button action that requires a target, or a teleport block cell, click on any cell to set this cell as the target.

Active mode: While this is toggled on, all changes of the cell property controls will instantly apply to all selected cells. This lets you quickly set cell properties without having to maunally paint or fill cells in the map view. Use with caution!

Show water: Shows or hides water overlay icons. Useful when editing maps with lots of water cells.

Auto stairs: When toggled on, making a cell a stair block automatically applies the corresponding stair auxiliary property to the adjacent cell.

Show sky: When toggled on, a dark grey overlay shows where the sky/horizon is not rendered for each of the four viewing directions for all cells of the current Z level.

Editing filter: Only the cell properties chosen will be applied when editing.

Debug: Shows a debug panel. Only used for development of the editor.

OPTIONS

Show extended curosor info: Displays info for the cell under the cursor, such as button actions, item id, and conversations.

CELL CONTROLS

Fill water: Apply the selected water cell properties to all cells on the current Z level that have no block and are not the target of a button with a button action that removes the cell.

FINISHING A MAP

The map must be placed inside the Maps directory of the Hired Guns (CD32) folder and have a two-digit number as it's file name. The game will load both compressed and uncompressed maps. Note that the HGViewer program from the development files will only open compressed maps. The map's file number must correspond to the file name (it can be single-digit, though). You can check and set the file number in the Map property panel. 

Before you can play your custom map properly, you need to create the Locations.dat file, which will be placed into the World directory of the Hired Guns folder and will overwrite an existing Locations.dat file. Clicking Tools/Create Locations.dat makes the set up and progress window pop up. Set it up according to the information on the right. Click the Go! button on the bottom, and the Locations.dat file will be created.
