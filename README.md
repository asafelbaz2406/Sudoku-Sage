# Sudoku-Sage
Challenge your mind and sharpen your logic with Sudoku Sage! With 200 carefully crafted Sudoku grids, ranging from easy to expert levels, this sleek and interactive 2D experience is designed for puzzle lovers of all skill levels.

I used a backtracking algorithm to create the Sudoku maps, storing solutions in a matrix. I then removed a set number of cells while ensuring there was only one valid solution for each puzzle. These removed cells are saved in a dictionary with Point(i, j) as keys and int removedCell as values.

Initially, I implemented the levels using Scriptable Objects, but switched to a JSON file midway through development for better flexibility. I then created a SudokuDataContainer class to store the solution board, playable board (after cell removal), deleted positions dictionary, and the map level.

Keeping the code clean and organized allowed me to fix bugs and add new features with ease.

Overall, building this game was a fun and rewarding experience! :)
