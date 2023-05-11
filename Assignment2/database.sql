DROP TABLE IF EXISTS `Patients`;
DROP TABLE IF EXISTS `Users`;

CREATE TABLE `Users` (
  `Id` varchar(40) NOT NULL,
  `Email` varchar(30) NOT NULL,
  `Password` varchar(100) NOT NULL,
  `Key2FA` varchar(12) NOT NULL,
  `Role` varchar(10) NOT NULL COMMENT 'doctor/patient',
  PRIMARY KEY(`Id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

CREATE TABLE `Patients` (
  `Id` varchar(40) NOT NULL,
  `FirstName` varchar(30) NOT NULL,
  `LastName` varchar(30) NOT NULL,
  `Gender` varchar(6) NOT NULL COMMENT 'male/female/other',
  `Birthdate` date NOT NULL,
  `Address` varchar(40) NOT NULL,
  `Phone` varchar(15) NOT NULL,
  `Terms` tinyint(1) NOT NULL,
  `ProfilePicture` varchar(200) NOT NULL DEFAULT 'https://upload.wikimedia.org/wikipedia/commons/1/14/No_Image_Available.jpg',
  PRIMARY KEY(`Id`),
  FOREIGN KEY (`Id`) REFERENCES `Users` (`Id`) ON DELETE CASCADE ON UPDATE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;

INSERT INTO `Users` (`Id`, `Email`, `Password`, `Role`, `Key2FA`) VALUES
('53379ed0-de3b-40d5-b535-ee15119df05f', 'davide@albiero.it', '$2a$11$VkcJxoFFGlXXDoxYp3x5keiyrTxnh/n32JMVL8.hedMRBPmJfwjlS', 'doctor', ''),
('6fbe6a58-dcb0-422e-af55-51f072cb7eca', 'tina.anselmi@gmail.com', '$2a$11$hCQ4P9NAWT9YWebagsfxguwutLQ.zbS5AcnmcF6.gG4HtnCrbTjly', 'patient', ''),
('cc1031a6-cfff-43c0-af19-b38eab358bdc', 'mario.rossi@gmail.com', '$2a$11$CGxLyWV7/aS0bbLOVhdcduYTHqT4c33isV25M7yNBBRIXwOFsZSwC', 'patient', ''),
('cc7d74b3-f798-42a3-bce5-a4ac9a534c14', 'giuseppe.mazzini@gmail.com', '$2a$11$gnGwKpgavDFfXuBqybD7POLck7NuV5moqwIN/Pbu132TfecfUwC6e', 'patient', '');

INSERT INTO `Patients` (`Id`, `FirstName`, `LastName`, `Gender`, `Birthdate`, `Address`, `Phone`, `Terms`, `ProfilePicture`) VALUES
('6fbe6a58-dcb0-422e-af55-51f072cb7eca', 'Tina', 'Anselmi', 'Female', '1927-03-25', 'Via G.Verdi', '+39 3453829984', 1, 'https://shdfiles.blob.core.windows.net/tinaanselmi/c6932ef9-d5d9-435f-99c0-42b969a7a955.jpg'),
('cc1031a6-cfff-43c0-af19-b38eab358bdc', 'Mario', 'Rossi', 'Male', '1999-10-15', 'Via. A. Rossi', '+39 3547282733', 1, 'https://shdfiles.blob.core.windows.net/mariorossi/869586a4-e7e6-4009-8802-a83facc7346e.jpg'),
('cc7d74b3-f798-42a3-bce5-a4ac9a534c14', 'Giuseppe', 'Mazzini', 'Male', '1860-03-29', 'Via M. Giove', '+39 4930284031', 0, 'https://shdfiles.blob.core.windows.net/giuseppemazzini/ed144ff8-85a0-4bc4-a88a-4176cb7e4abf.jpg');
