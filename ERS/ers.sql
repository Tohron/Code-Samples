INSERT INTO user_roles (user_role) VALUES ('EMPLOYEE'), ('FINANCE');
INSERT INTO reimbursement_type (reimb_type) VALUES ('BUSINESS'), ('TRAVEL'), ('PARKING'), ('OTHER');
INSERT INTO reimbursement_status (reimb_status) VALUES ('APPROVED'), ('REJECTED');
INSERT INTO users (ers_username, ers_password, user_first_name, user_last_name, user_email, user_role_id) 
	VALUES ('FredH', 'notdefault', 'Fred', 'Harrison', 'fharrision@portal.com', 1), 
			('TheBob', 'thepassword', 'Bob', 'Amaris', 'notthatbob@gmail.com', 1),
			('MrJohnson', 'security', 'Bill', 'Johnson', 'bjohnson@comcast.net', 2);
INSERT INTO reimbursement (reimb_amount, reimb_submitted, reimb_resolved, reimb_author, reimb_resolver, reimb_status_id, reimb_type_id) 
	VALUES (12.32, '2004-04-01 13:05:00', NULL, 9, NULL, NULL, 4),
			(62.21, '2009-04-01 18:55:20', NULL, 9, NULL, NULL, 3),
			(462.21, '1959-04-01 03:45:20', NULL, 9, NULL, NULL, 4),
			(362.65, '1959-04-01 09:15:20', NULL, 10, NULL, NULL, 3),
			(262.65, '1985-11-21 11:16:24', '2095-12-03 21:16:24', 9, 11, 7, 3),
			(262.65, '1989-11-01 12:16:24', '2045-12-03 23:16:24', 10, 11, 8, 5);