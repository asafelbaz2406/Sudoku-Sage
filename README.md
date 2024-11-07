# Sudoku-Sage

Challenge your mind and sharpen your logic with Sudoku Sage! With 200 carefully crafted Sudoku grids, ranging from easy to expert levels, this sleek and interactive 2D experience is designed for puzzle lovers of all skill levels.

I used a backtracking algorithm to generate the Sudoku puzzles, storing the solutions in a matrix. Then, I removed a set number of cells while ensuring there was only one valid solution for each puzzle. These removed cells are saved in a dictionary with Point(i, j) as keys and int removedCell as values, enabling a super easy hint system. I also used a stack to implement a simple undo feature.

Initially, I used Scriptable Objects to implement the levels but switched to a JSON file midway for better flexibility. To manage this, I created a SudokuDataContainer class that stores the solution board, the playable board (after cell removal), the deleted positions dictionary, and the map level.

Keeping the code clean and organized made it easy to fix bugs and add new features along the way.

Overall, building this game was a fun and rewarding experience! :)
