# EdhesiveAggregator
Consolidate quizzes and assignments from Edhesive AP CS a course

Usage: EdhesiveAggregator <file.csv>

New aggreated CSV file is output to Output subdirectory.

Completes the following batch tasks:
* Removes empty columns
* Removes generated columns
* Removes "ID", "SYS User ID", "SYS Login ID" and "Section" columns
* Aggregates all Fast Start quizzes by unit
* Aggregates all Review Questions quizzes by unit
* Aggregates all Coding Activity assignments by unit

What results is a much cleaner grade book that's easily entered in Synergy
