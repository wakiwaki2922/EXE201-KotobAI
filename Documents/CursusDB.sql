-- Create the database
CREATE DATABASE IF NOT EXISTS cursusdb;

-- Use the database
USE cursusdb;

-- 1. Major
CREATE TABLE `Major` (
  `id` int,
  `description` text(2147483647),
  PRIMARY KEY (`id`)
);

-- 2. Role
CREATE TABLE `Role` (
  `id` int,
  `name` varchar(50),
  PRIMARY KEY (`id`)
);

-- 3. User
CREATE TABLE `User` (
  `id` int,
  `name` varchar(100),
  `email` varchar(100),
  `role_id` int,
  PRIMARY KEY (`id`),
  KEY `AK` (`email`),
  FOREIGN KEY (`role_id`) REFERENCES `Role`(`id`)
);

-- 4. Dictionary
CREATE TABLE `Dictionary` (
  `id` int,
  `languague` varchar(50),
  PRIMARY KEY (`id`)
);

-- 5. Vocab
CREATE TABLE `Vocab` (
  `id` int,
  `kanji` varchar(50),
  `hiragana` varchar(50),
  `romaji` varchar(50),
  `vietnamese` text(2147483647),
  `pronoun` varchar(100),
  `example` text(2147483647),
  PRIMARY KEY (`id`)
);

-- 6. Collection
CREATE TABLE `Collection` (
  `id` int,
  `name` varchar(100),
  PRIMARY KEY (`id`)
);

-- 7. Sample_Collection
CREATE TABLE `Sample_Collection` (
  `id` int,
  `major_id` int,
  `name` varchar(100),
  PRIMARY KEY (`id`),
  FOREIGN KEY (`major_id`) REFERENCES `Major`(`id`)
);

-- 8. Lesson
CREATE TABLE `Lesson` (
  `id` int,
  `major_id` int,
  `collection_id` int,
  `sample_collection_id` int,
  `description` text(2147483647),
  PRIMARY KEY (`id`),
  FOREIGN KEY (`major_id`) REFERENCES `Major`(`id`),
  FOREIGN KEY (`collection_id`) REFERENCES `Collection`(`id`),
  FOREIGN KEY (`sample_collection_id`) REFERENCES `Sample_Collection`(`id`)
);

-- 9. Subscription
CREATE TABLE `Subscription` (
  `id` int,
  `user_id` int,
  `plan` varchar(50),
  `start_date` date,
  `end_date` date,
  `status` varchar(20),
  PRIMARY KEY (`id`),
  FOREIGN KEY (`user_id`) REFERENCES `User`(`id`)
);

-- 10. Payment
CREATE TABLE `Payment` (
  `id` int,
  `user_id` int,
  `amount` decimal,
  `status` varchar(20),
  `payment_date` date,
  PRIMARY KEY (`id`),
  FOREIGN KEY (`user_id`) REFERENCES `User`(`id`)
);

-- 11. User_Sample_Collection
CREATE TABLE `User_Sample_Collection` (
  `user_id` int,
  `sample_collection_id` int,
  PRIMARY KEY (`user_id`, `sample_collection_id`),
  FOREIGN KEY (`user_id`) REFERENCES `User`(`id`),
  FOREIGN KEY (`sample_collection_id`) REFERENCES `Sample_Collection`(`id`)
);

-- 12. User_Collection
CREATE TABLE `User_Collection` (
  `user_id` int,
  `collection_id` int,
  PRIMARY KEY (`user_id`, `collection_id`),
  FOREIGN KEY (`user_id`) REFERENCES `User`(`id`),
  FOREIGN KEY (`collection_id`) REFERENCES `Collection`(`id`)
);

-- 13. Sample_Collection_Vocab
CREATE TABLE `Sample_Collection_Vocab` (
  `sample_collection_id` int,
  `vocab_id` int,
  PRIMARY KEY (`sample_collection_id`, `vocab_id`),
  FOREIGN KEY (`sample_collection_id`) REFERENCES `Sample_Collection`(`id`),
  FOREIGN KEY (`vocab_id`) REFERENCES `Vocab`(`id`)
);

-- 14. Collection_Vocab
CREATE TABLE `Collection_Vocab` (
  `collection_id` int,
  `vocab_id` int,
  PRIMARY KEY (`collection_id`, `vocab_id`),
  FOREIGN KEY (`collection_id`) REFERENCES `Collection`(`id`),
  FOREIGN KEY (`vocab_id`) REFERENCES `Vocab`(`id`)
);

-- 15. Dictionary_Vocab
CREATE TABLE `Dictionary_Vocab` (
  `dictionary_id` int,
  `vocab_id` int,
  PRIMARY KEY (`dictionary_id`, `vocab_id`),
  FOREIGN KEY (`dictionary_id`) REFERENCES `Dictionary`(`id`),
  FOREIGN KEY (`vocab_id`) REFERENCES `Vocab`(`id`)
);

-- 16. User_Dictionary
CREATE TABLE `User_Dictionary` (
  `user_id` int,
  `dictrionary_id` int,
  PRIMARY KEY (`user_id`, `dictrionary_id`),
  FOREIGN KEY (`user_id`) REFERENCES `User`(`id`),
  FOREIGN KEY (`dictrionary_id`) REFERENCES `Dictionary`(`id`)
);