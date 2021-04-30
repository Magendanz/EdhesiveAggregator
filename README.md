# EdhesiveAggregator
Consolidate quizzes and assignments from Edhesive AP CS A course

Usage: EdhesiveAggregator <file.csv>

New aggregated CSV file is output to Output subdirectory.

Completes the following batch tasks:
* Removes empty columns
* Removes generated columns
* Removes "ID", "SYS User ID", "SYS Login ID" and "Section" columns
* Aggregates all Fast Start quizzes by unit
* Aggregates all Review Questions quizzes by unit
* Aggregates all Coding Activity assignments by unit
* Aggregates all Labs
* Aggregates all multipart FRQs
* Automatically opens new file in Excel

What results is a much cleaner grade book that's easily entered in Synergy
