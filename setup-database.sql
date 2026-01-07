-- Create the database user
CREATE USER cinema_user WITH PASSWORD 'Cinema123!';

-- Create the database
CREATE DATABASE cinema_cafeteria OWNER cinema_user;

-- Grant necessary privileges
GRANT ALL PRIVILEGES ON DATABASE cinema_cafeteria TO cinema_user;

-- Connect to the database and grant schema privileges
\c cinema_cafeteria
GRANT ALL ON SCHEMA public TO cinema_user;

