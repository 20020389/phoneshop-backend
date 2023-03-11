USE $DATABASE$
DROP TRIGGER IF EXISTS auto_create_cart;
DELIMITER $$

CREATE TRIGGER auto_create_cart
    AFTER INSERT
    ON user

    FOR EACH ROW

BEGIN
    INSERT cart

    SET userId = NEW.uid;

END $$;

DROP TRIGGER IF EXISTS auto_delete_cart;

DELIMITER $$

CREATE TRIGGER auto_delete_cart
    BEFORE DELETE
    ON user

    FOR EACH ROW

BEGIN
    DELETE FROM cart WHERE cart.userId = OLD.uid;
END $$;

-- trigger for auto set rating in phone
DROP TRIGGER IF EXISTS auto_phone_rating_0;
DELIMITER $$
CREATE TRIGGER auto_phone_rating
    AFTER INSERT
    ON phonerating

    FOR EACH ROW

BEGIN
    SET @count = (SELECT (SUM(pt.ratingValue) / COUNT(pt.id)) as value
                  FROM phonerating pt
                  WHERE phoneId = NEW.phoneId
                  GROUP BY phoneId);
    UPDATE phone
    SET rating = @count
    WHERE uid = NEW.phoneId;
END $$;

DROP TRIGGER IF EXISTS auto_phone_rating_1;
DELIMITER $$
CREATE TRIGGER auto_phone_rating
    AFTER UPDATE
    ON phonerating

    FOR EACH ROW

BEGIN
    SET @count = (SELECT (SUM(pt.ratingValue) / COUNT(pt.id)) as value
                  FROM phonerating pt
                  WHERE phoneId = NEW.phoneId
                  GROUP BY phoneId);
    UPDATE phone
    SET rating = @count
    WHERE uid = NEW.phoneId;
END $$;

DROP TRIGGER IF EXISTS auto_phone_rating_2;
DELIMITER $$
CREATE TRIGGER auto_phone_rating
    AFTER DELETE
    ON phonerating

    FOR EACH ROW

BEGIN
    SET @count = (SELECT (SUM(pt.ratingValue) / COUNT(pt.id)) as value
                  FROM phonerating pt
                  WHERE phoneId = OLD.phoneId
                  GROUP BY phoneId);
    UPDATE phone
    SET rating = @count
    WHERE uid = OLD.phoneId;
END $$;

# xóa phonerating và phoneoffer khi xóa phone
DROP TRIGGER IF EXISTS auto_delete_phonedata;

DELIMITER $$

CREATE TRIGGER auto_delete_phonedata
    BEFORE DELETE
    ON phone

    FOR EACH ROW

BEGIN
    DELETE FROM phonerating WHERE phoneId = OLD.uid;
    DELETE FROM phoneoffer WHERE phoneId = OLD.uid;
END $$;

#