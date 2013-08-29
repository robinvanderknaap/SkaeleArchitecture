DROP TABLE IF EXISTS Log;

CREATE TABLE Log
(
	Id uuid PRIMARY KEY,
	Created timestamp NOT NULL,
	Level varchar(10) NOT NULL,
	Environment varchar(20) NOT NULL,
	Source varchar(255) NOT NULL,
	Message varchar(255) NOT NULL,
	Details varchar(255) NULL,
	Username varchar(70) NULL,
	RequestMethod varchar(10) NULL,
	RequestUrl varchar(255) NULL,
	UrlReferrer varchar(255) NULL,
	ClientBrowser varchar(255) NULL,
	IpAddress varchar(100) NULL,
	PostedFormValues text NULL,
	Stacktrace text NOT NULL,
	Exception text NULL
);