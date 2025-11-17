create database QLTV;
go
use QLTV;
go

-- Bảng người dùng với ID bắt đầu từ 1000
create table NguoiDung(
    ID int primary key identity(1,1),  -- Bắt đầu từ 1000 và tăng dần
    TenDangNhap varchar(50) unique,
    MatKhau varbinary(MAX),
    Email varchar(50) unique,
    NgayDangKi datetime,
    MaOTP varchar(6),
    ThoiGianNhanOTP datetime,
    TrangThaiXacThuc bit default 0,
    QuyenHan bit default(0), -- 0 là user, 1 là admin
    HoTen nvarchar(50),
	RandomKey VARCHAR(50)
);
go

-- Bảng tác giả
create table TacGia(
    MaTG int primary key identity(1,1),
    TenTG nvarchar(50),
    MoTa nvarchar(500),
    SoSach int
);
go

-- Bảng nhà xuất bản
create table NhaXuatBan(
    MaNXB int primary key identity(1,1),
    TenNXB nvarchar(50),
    MoTa nvarchar(500)
);
go

-- Bảng thể loại
create table TheLoai(
    MaTheLoai int primary key identity(1,1),
    TenTheLoai nvarchar(50),
    MoTa nvarchar(500),
    SoSach int
);
go

-- Bảng sách
create table Sach(
    ID int primary key identity(1,1),  -- ID sách do người dùng nhập vào
    TenSach nvarchar(100),
    MaTG int foreign key (MaTG) references TacGia(MaTG),
    MaNXB int foreign key (MaNXB) references NhaXuatBan(MaNXB),
    MaTheLoai int foreign key (MaTheLoai) references TheLoai(MaTheLoai),    
    SoLuong int default 1,
    MoTa nvarchar(500)
);
go

-- Bảng phiếu mượn
create table PhieuMuon(
    MaPhieu int primary key identity(1,1),
    IDBanDoc int foreign key (IDBanDoc) references NguoiDung(ID),
    IDSach int foreign key (IDSach) references Sach(ID),
    SoLuong int,
    NgayDangKyMuon datetime,
    NgayMuon datetime,
    HanTra datetime,
    NgayTra datetime,
    TrangThai int default 0  -- 0 = Chưa mượn, 1 = Đang mượn, 2 = Quá Hạn, 3 = Đã Trả
);
go

-- Thêm vào bảng NguoiDung (Người dùng)
INSERT INTO NguoiDung (TenDangNhap, MatKhau, Email, NgayDangKi, MaOTP, ThoiGianNhanOTP, TrangThaiXacThuc, QuyenHan, HoTen, RandomKey)
VALUES
('a', 0x158CA628A01A9AAC19A4FFA80E7EA86D, 'huynhmyduc2005@gmail.com', '12/14/2024 3:04:45 AM',	'2899',	'12/15/2024 2:53:25 AM', 1, 0, 'a',	'7573'),
('aa', 0xF2ACEC901B87B259BEB8C8E245EC48C2,	'kivatkamenrider@gmail.com', '12/14/2024 3:07:38 AM', '8764', '12/14/2024 3:07:22 AM', 1, 0, 'Duck', '8331'),
('duck', 0x6201090A4CF6E501F9F0D7A1C75C584D, 'huynhmyduc2020@gmail.com', '12/15/2024 12:30:00 PM', '4566', NULL, 0,	0, 'Test Acc', '4566'),
('admin', 0xCCDDB19D94622D5FF1EE354D99D24D20, 'duckidgaming2005@gmail.com',	'12/15/2024 2:28:54 PM', '5485', '12/15/2024 2:41:51 PM', 1, 1,	'Admin', '5485')
GO


-- Thêm vào bảng TacGia (Tác giả)
INSERT INTO TacGia (TenTG, MoTa, SoSach)
VALUES
('Nguyen Van A', N'Tác giả chuyên viết sách về khoa học tự nhiên.', 10),
('Nguyen Thi B', N'Tác giả chuyên viết sách về văn học.', 15),
('Le Minh C', N'Tác giả nổi bật trong lĩnh vực lịch sử.', 8),
('Tran Thi D', N'Tác giả với nhiều tác phẩm về vật lý.', 5),
('Pham Minh E', N'Chuyên gia về sinh học và nghiên cứu về môi trường.', 12);
GO

-- Thêm vào bảng NhaXuatBan (Nhà xuất bản)
INSERT INTO NhaXuatBan (TenNXB, MoTa)
VALUES
(N'NXB Giáo Dục', N'Nhà xuất bản chuyên về sách giáo khoa và sách giáo dục.'),
(N'NXB Văn Học', N'Nhà xuất bản chuyên về các tác phẩm văn học nổi tiếng.'),
(N'NXB Khoa Học', N'Nhà xuất bản chuyên phát hành các sách nghiên cứu khoa học.'),
(N'NXB Lịch Sử', N'Nhà xuất bản chuyên về các sách lịch sử.'),
(N'NXB Ngoại Ngữ', N'Nhà xuất bản chuyên phát hành sách học ngoại ngữ.');
GO

-- Thêm vào bảng TheLoai (Thể loại sách)
INSERT INTO TheLoai (TenTheLoai, MoTa, SoSach)
VALUES
(N'Khoa Học', N'Các sách liên quan đến nghiên cứu khoa học.', 30),
(N'Văn Học', N'Các sách thuộc thể loại văn học.', 40),
(N'Lịch Sử', N'Các sách về lịch sử các quốc gia và sự kiện lịch sử.', 25),
(N'Ngoại Ngữ', N'Sách học ngoại ngữ như Tiếng Anh, Tiếng Pháp...', 35),
(N'Toán Học', N'Các sách học về toán và lý thuyết toán học.', 20);
GO
	
-- Thêm vào bảng Sach (Sách)
INSERT INTO Sach (TenSach, MaTG, MaNXB, MaTheLoai, SoLuong, MoTa)
VALUES
(N'Toán Cơ Bản', 1, 1, 5, 10, N'Giới thiệu các kiến thức cơ bản trong toán học.'),
(N'Văn Học Việt Nam', 2, 2, 2, 15, N'Tổng hợp các tác phẩm văn học nổi bật của Việt Nam.'),
(N'Lịch Sử Việt Nam', 3, 4, 3, 20, N'Sách nghiên cứu về lịch sử Việt Nam qua các thời kỳ.'),
(N'Vật Lý Đại Cương', 4, 3, 1, 8, N'Tổng hợp các kiến thức vật lý cơ bản.'),
(N'Sinh Học Tự Nhiên', 5, 5, 1, 25, N'Các kiến thức về sinh học và nghiên cứu sinh thái học.'),
(N'Tiếng Anh Giao Tiếp', 2, 5, 4, 30, N'Hướng dẫn học tiếng Anh giao tiếp cơ bản.'),
(N'Lập Trình Cơ Bản', 1, 3, 1, 15, N'Giới thiệu các khái niệm lập trình cơ bản cho người mới bắt đầu.'),
(N'Hóa Học Cơ Bản', 4, 3, 1, 10, N'Những nguyên lý cơ bản trong hóa học.'),
(N'Quản Trị Doanh Nghiệp', 2, 2, 5, 18, N'Sách về các phương pháp quản trị doanh nghiệp hiệu quả.'),
(N'Kinh Tế Vi Mô', 5, 4, 3, 12, N'Các nguyên lý cơ bản của kinh tế vi mô.');
GO

-- Thêm vào bảng PhieuMuon (Phiếu mượn sách)
INSERT INTO PhieuMuon (IDBanDoc, IDSach, SoLuong, NgayDangKyMuon, NgayMuon, HanTra, NgayTra, TrangThai)
VALUES
(1, 1, 2, '2024-12-17', NULL, NULL, NULL, 0),
(2, 2, 5, '2024-12-17', NULL, NULL, NULL, 0),
(1, 3, 1, '2024-12-16', NULL, NULL, NULL, 0),
(4, 4, 3, '2024-12-15', NULL, NULL, NULL, 0),
(4, 5, 1, '2024-03-01', '2024-03-05', '2024-04-05', NULL, 2),
(1, 2, 2, '2024-03-01', '2024-03-05', '2024-04-05', NULL, 2),
(2, 3, 3, '2024-04-01', '2024-04-05', '2024-05-05', '2024-04-25', 3),
(4, 4, 1, '2024-05-01', '2024-05-05', '2024-06-05', NULL, 2),
(2, 5, 2, '2024-06-01', '2024-06-05', '2024-07-05', '2024-06-20', 3),
(2, 1, 1, '2024-07-01', '2024-07-05', '2024-08-05', NULL, 2),
(1, 2, 2, '2024-08-01', '2024-08-05', '2024-12-20', NULL, 1),
(4, 3, 1, '2024-09-01', '2024-09-05', '2024-10-05', '2024-09-25',3),
(3, 4, 3, '2024-10-01', '2024-10-05', '2024-11-05', '2024-10-20', 3),
(1, 5, 2, '2024-11-01', '2024-11-05', '2024-12-05', NULL, 2);
GO
