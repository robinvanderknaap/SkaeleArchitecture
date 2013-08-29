ALTER TABLE Users ADD Password varchar(200) GO
ALTER TABLE Users ADD LoginAttempts int GO
ALTER TABLE Users ADD LastLoginAttempt timestamp GO
ALTER TABLE Users ADD NewPasswordRequested timestamp GO
ALTER TABLE Users ADD NewPasswordRequestedHash varchar(255) GO