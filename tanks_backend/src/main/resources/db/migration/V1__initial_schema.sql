CREATE SCHEMA IF NOT EXISTS game;

CREATE TABLE IF NOT EXISTS game.user (
                                         id integer PRIMARY KEY,
                                         username varchar UNIQUE NOT NULL,
                                         password_hash varchar NOT NULL,
                                         email varchar UNIQUE NOT NULL,
                                         role varchar DEFAULT 'PLAYER',
                                         status varchar DEFAULT 'ACTIVE',
                                         created_at timestamp,
                                         updated_at timestamp,
                                         last_login timestamp
);

CREATE TABLE IF NOT EXISTS game.leaderboard (
                                        id integer PRIMARY KEY,
                                        user_id integer UNIQUE,
                                        wins integer DEFAULT 0,
                                        losses integer DEFAULT 0,
                                        kills integer DEFAULT 0,
                                        deaths integer DEFAULT 0,
                                        matches_played integer DEFAULT 0,
                                        created_at timestamp,
                                        updated_at timestamp
);

CREATE TABLE IF NOT EXISTS game.team (
                                         id integer PRIMARY KEY,
                                         name varchar UNIQUE NOT NULL,
                                         created_at timestamp,
                                         updated_at timestamp
);

CREATE TABLE IF NOT EXISTS game.game_session (
                                         id integer PRIMARY KEY,
                                         name          varchar UNIQUE NOT NULL,
                                         password      varchar,
                                         status varchar,
                                         winner_team_id integer,
                                         start_time timestamp,
                                         end_time timestamp,
                                         game_settings jsonb,
                                         summary jsonb,
                                         created_at timestamp,
                                         updated_at timestamp
);

CREATE TABLE IF NOT EXISTS game.game_session_player (
                                        id integer PRIMARY KEY,
                                        player_id integer,
                                        game_session_id integer,
                                        team_id integer,
                                        kills integer DEFAULT 0,
                                        deaths integer DEFAULT 0,
                                        joined_at timestamp,
                                        left_at timestamp
);

CREATE TABLE IF NOT EXISTS game.game_events (
                                        id integer PRIMARY KEY,
                                        game_session_id integer,
                                        event_type varchar,
                                        event_time timestamp,
                                        summary jsonb
);

COMMENT ON COLUMN game.user.status IS 'ACTIVE, BANNED, INACTIVE';

COMMENT ON COLUMN game.game_session.status IS 'ACTIVE, FINISHED';

COMMENT ON COLUMN game.game_session.summary IS 'Aggregated game stats';

COMMENT ON COLUMN game.game_events.event_type IS 'e.g., GAME_START, PLAYER_HIT';

COMMENT ON COLUMN game.game_events.summary IS 'Event details';

ALTER TABLE game.game_session ADD FOREIGN KEY (winner_team_id) REFERENCES game.team (id);

ALTER TABLE game.game_session_player ADD FOREIGN KEY (game_session_id) REFERENCES game.game_session (id);

ALTER TABLE game.game_session_player ADD FOREIGN KEY (player_id) REFERENCES game.user (id);

ALTER TABLE game.game_session_player ADD FOREIGN KEY (team_id) REFERENCES game.team (id);

ALTER TABLE game.game_events ADD FOREIGN KEY (game_session_id) REFERENCES game.game_session (id);

ALTER TABLE game.user ADD FOREIGN KEY (id) REFERENCES game.leaderboard (user_id);