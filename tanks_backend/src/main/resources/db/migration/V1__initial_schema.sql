CREATE SCHEMA IF NOT EXISTS game;

CREATE TABLE IF NOT EXISTS game.user (
                                         id BIGSERIAL PRIMARY KEY,
                                         username varchar UNIQUE NOT NULL,
                                         password_hash varchar NOT NULL,
                                         email varchar NOT NULL,
                                         role varchar DEFAULT 'PLAYER',
                                         status varchar DEFAULT 'ACTIVE',
                                         created_at timestamp,
                                         updated_at timestamp,
                                         last_login timestamp
);

CREATE TABLE IF NOT EXISTS game.user_stats (
                                                id      BIGSERIAL PRIMARY KEY,
                                                user_id bigint UNIQUE,
                                        wins integer DEFAULT 0,
                                        losses integer DEFAULT 0,
                                        kills integer DEFAULT 0,
                                        deaths integer DEFAULT 0,
                                        matches_played integer DEFAULT 0,
                                        created_at timestamp,
                                        updated_at timestamp
);

CREATE TABLE IF NOT EXISTS game.game_session (
                                                 id             BIGSERIAL PRIMARY KEY,
                                                 host_id bigint,
                                                 name           varchar UNIQUE NOT NULL,
                                                 password       varchar,
                                         max_players integer,
                                         status varchar,                                   
                                         start_time timestamp,
                                         end_time timestamp,
                                         summary jsonb,
                                         created_at timestamp,
                                         updated_at timestamp
);

CREATE TABLE IF NOT EXISTS game.game_session_player (
                                                        id              BIGSERIAL PRIMARY KEY,
                                                        player_id       bigint,
                                                        game_session_id bigint,
                                        kills integer DEFAULT 0,
                                        deaths integer DEFAULT 0,
                                        joined_at timestamp,
                                        left_at timestamp
);

CREATE TABLE IF NOT EXISTS game.game_events (
                                                id              BIGSERIAL PRIMARY KEY,
                                                game_session_id bigint,
                                        event_type varchar,
                                        event_time timestamp,
                                        event_summary varchar
                
);

COMMENT ON COLUMN game.user.status IS 'ACTIVE, BANNED, INACTIVE';

COMMENT ON COLUMN game.game_session.status IS 'ACTIVE, FINISHED';

COMMENT ON COLUMN game.game_session.summary IS 'Aggregated game stats';

COMMENT ON COLUMN game.game_events.event_type IS 'e.g., GAME_START, WAVE_START, WAVE_END, GAME_END etc.';


ALTER TABLE game.game_session_player ADD FOREIGN KEY (game_session_id) REFERENCES game.game_session (id);

ALTER TABLE game.game_session_player ADD FOREIGN KEY (player_id) REFERENCES game.user (id);


ALTER TABLE game.game_events ADD FOREIGN KEY (game_session_id) REFERENCES game.game_session (id);

ALTER TABLE game.user_stats
    ADD FOREIGN KEY (user_id) REFERENCES game.user (id);