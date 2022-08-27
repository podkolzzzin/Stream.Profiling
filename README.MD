# Implementation of External Merge Sort
Full Stream: https://www.youtube.com/watch?v=B9v7pdfhUYw
## Satellite videos:
1. Task Implementation: https://youtu.be/8XIMleu0YWI
1. Span: https://youtu.be/3QDhMR3ljcM
1. async/await problems: https://youtu.be/sQk6XW9xLQQ

# 2000 years later I have updated this repo using optimizations from user comments:
The video describing optimizations: https://youtu.be/MX3R2316nV4

Program output on my PC on 20_000_000 lines(1.5GB):
```
Generated
>>Insert took: 00:00:17.34
>>Sorting Elapsed took: 00:00:37.43
Total using SQLite: 00:00:54.76
Custom solution: 00:00:40.67
Ultimate solution: 00:00:10.73
Ultimate solution tuned: 00:00:10.10
Ultimate solution 3: 00:00:20.28
Ultimate solution 3 Tuned: 00:00:20.31
```

Program output on my PC on 400_000_000 lines:
```
Generated
>>Insert took: 00:07:09.7968890
>>Sorting Elapsed took: 00:25:48.2921843
Total using SQLite: 00:32:58.0913111 // 28.5GB db size
Custom solution: 00:24:37.7879316 // 20_000_000 lines in batch
Ultimate solution tuned: 00:06:18.0619101
```