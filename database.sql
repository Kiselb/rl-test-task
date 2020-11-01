USE [master]
GO
/****** Object:  Database [RLTEST]    Script Date: 01/11/20 14:13:54 ******/
CREATE DATABASE [RLTEST]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'RLTEST', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\RLTEST.mdf' , SIZE = 5120KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'RLTEST_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL11.MSSQLSERVER\MSSQL\DATA\RLTEST_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [RLTEST] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [RLTEST].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [RLTEST] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [RLTEST] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [RLTEST] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [RLTEST] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [RLTEST] SET ARITHABORT OFF 
GO
ALTER DATABASE [RLTEST] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [RLTEST] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [RLTEST] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [RLTEST] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [RLTEST] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [RLTEST] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [RLTEST] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [RLTEST] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [RLTEST] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [RLTEST] SET  DISABLE_BROKER 
GO
ALTER DATABASE [RLTEST] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [RLTEST] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [RLTEST] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [RLTEST] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [RLTEST] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [RLTEST] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [RLTEST] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [RLTEST] SET RECOVERY FULL 
GO
ALTER DATABASE [RLTEST] SET  MULTI_USER 
GO
ALTER DATABASE [RLTEST] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [RLTEST] SET DB_CHAINING OFF 
GO
ALTER DATABASE [RLTEST] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [RLTEST] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [RLTEST]
GO
/****** Object:  User [webuser]    Script Date: 01/11/20 14:13:54 ******/
CREATE USER [webuser] FOR LOGIN [webuser] WITH DEFAULT_SCHEMA=[dbo]
GO
/****** Object:  Table [dbo].[Role]    Script Date: 01/11/20 14:13:54 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Role](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Role] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 01/11/20 14:13:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Login] [nvarchar](255) NOT NULL,
	[Name] [nvarchar](255) NOT NULL,
	[Email] [nvarchar](255) NOT NULL,
	[Password] [nvarchar](255) NOT NULL,
 CONSTRAINT [PK_Users] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[UserRole]    Script Date: 01/11/20 14:13:55 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRole](
	[UserId] [int] NOT NULL,
	[RoleId] [int] NOT NULL,
 CONSTRAINT [PK_UserRole] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC,
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_Role] FOREIGN KEY([RoleId])
REFERENCES [dbo].[Role] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_Role]
GO
ALTER TABLE [dbo].[UserRole]  WITH CHECK ADD  CONSTRAINT [FK_UserRole_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[UserRole] CHECK CONSTRAINT [FK_UserRole_User]
GO
USE [master]
GO
ALTER DATABASE [RLTEST] SET  READ_WRITE 
GO
USE [RLTEST]
GO
SET IDENTITY_INSERT dbo.Role ON
SET IDENTITY_INSERT dbo.[User] ON
INSERT INTO dbo.Role (Id, [Name]) VALUES (1, N'Admin')
INSERT INTO dbo.[User] (Id, [Login], [Name], [Email], [Password]) VALUES (1, N'Admin', N'Ivan Ivanov', N'IvanIvanov@IvanIvanov.ru', N'0123456789')
INSERT INTO dbo.UserRole (UserId, RoleId) VALUES (1, 1)
SET IDENTITY_INSERT dbo.Role OFF
SET IDENTITY_INSERT dbo.[User] OFF
GO
