USE [PCMS_D]
GO
/****** Object:  Table [dbo].[Agreements]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Agreements](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](max) NULL,
	[contents] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL,
	[creater] [nvarchar](max) NULL,
	[modifieddate] [datetime] NOT NULL,
	[modifier] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Agreements] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Approvals]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Approvals](
	[privacyId] [int] NOT NULL,
	[status] [int] NOT NULL,
	[createdate] [datetime] NOT NULL,
	[creater] [nvarchar](max) NULL,
	[modifieddate] [datetime] NOT NULL,
	[modifier] [nvarchar](max) NULL,
	[message] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Approvals] PRIMARY KEY CLUSTERED 
(
	[privacyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Batches]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Batches](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL,
	[status] [int] NOT NULL,
	[message] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL,
	[creater] [nvarchar](max) NULL,
	[bound] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Batches] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Channels]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Channels](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL,
	[bound] [int] NOT NULL,
	[type] [int] NOT NULL,
	[athour] [int] NOT NULL,
	[usage] [bit] NOT NULL,
	[modifieddate] [datetime] NOT NULL,
	[host] [nvarchar](max) NULL,
	[account] [nvarchar](max) NULL,
	[pwd] [nvarchar](max) NULL,
	[path] [nvarchar](max) NULL,
	[action] [int] NOT NULL,
	[Instantrun] [bit] NOT NULL,
	[exportpath] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Channels] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Collections]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Collections](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](max) NULL,
	[ftpname] [nvarchar](max) NULL,
	[status] [int] NOT NULL,
	[createdate] [datetime] NOT NULL,
	[modifieddate] [datetime] NOT NULL,
	[channelId] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Collections] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Consents]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Consents](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[CONSENT_DATE] [datetime] NOT NULL,
	[CONSENT_SOURCE] [nvarchar](max) NULL,
	[CONSENT_TYPE] [nvarchar](max) NULL,
	[CONSENT_VERSION] [nvarchar](max) NULL,
	[privacy_ID] [int] NULL,
	[CONSENT_USE] [bit] NOT NULL,
	[CONSENT_TRUST] [bit] NOT NULL,
	[CONSENT_ABROAD] [bit] NOT NULL,
	[CONSENT_SIGN] [bit] NOT NULL,
 CONSTRAINT [PK_dbo.Consents] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[N360File]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[N360File](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[SOURCE] [nvarchar](max) NULL,
	[ACTION] [nvarchar](max) NULL,
	[WKP_ID] [nvarchar](max) NULL,
	[WKP_EXT_ID] [nvarchar](max) NULL,
	[IND_ID] [nvarchar](max) NULL,
	[IND_EXT_ID] [nvarchar](max) NULL,
	[ACT_STATUS] [nvarchar](max) NULL,
	[WKP_NAME] [nvarchar](max) NULL,
	[WKP_TEL] [nvarchar](max) NULL,
	[WKP_FAX] [nvarchar](max) NULL,
	[ZIP] [nvarchar](max) NULL,
	[PROVINCE] [nvarchar](max) NULL,
	[CITY] [nvarchar](max) NULL,
	[DONG] [nvarchar](max) NULL,
	[STREET] [nvarchar](max) NULL,
	[FULL_ADDR] [nvarchar](max) NULL,
	[IND_SP] [nvarchar](max) NULL,
	[TITLE] [nvarchar](max) NULL,
	[IND_LASTNAME] [nvarchar](max) NULL,
	[IND_FIRSTNAME] [nvarchar](max) NULL,
	[IND_FULL_NAME] [nvarchar](max) NULL,
	[GENDER] [nvarchar](max) NULL,
	[EMAIL] [nvarchar](max) NULL,
	[MOBILE] [nvarchar](max) NULL,
	[CONSENT_STATUS] [nvarchar](max) NULL,
	[CONSENT_DATE] [datetime] NOT NULL,
	[CONSENT_SOURCE] [nvarchar](max) NULL,
	[CONSENT_TYPE] [nvarchar](max) NULL,
	[CONSENT_VERSION] [nvarchar](max) NULL,
	[modified] [bit] NOT NULL,
	[createdate] [datetime] NOT NULL,
	[collectionId] [int] NOT NULL,
	[PrivacyId] [int] NOT NULL,
	[PCMSID] [nvarchar](max) NULL,
	[OneKey] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.N360File] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PcmsIds]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PcmsIds](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[KEY] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.PcmsIds] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PforceRXFiles]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PforceRXFiles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IND_ID] [nvarchar](max) NULL,
	[CONSENT_EMAIL] [nvarchar](max) NULL,
	[CONSENT_MOBILE] [nvarchar](max) NULL,
	[CONSENT_STATUS] [nvarchar](max) NULL,
	[CONSENT_DATE] [datetime] NOT NULL,
	[CONSENT_SOURCE] [nvarchar](max) NULL,
	[EXTRACT_DATE] [datetime] NOT NULL,
	[COUNTRY_CD] [nvarchar](max) NULL,
	[MODIFIED] [bit] NOT NULL,
	[createdate] [datetime] NOT NULL,
	[CollectionId] [int] NOT NULL,
	[privacyId] [int] NULL,
 CONSTRAINT [PK_dbo.PforceRXFiles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Privacies]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Privacies](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[IND_ID] [nvarchar](max) NULL,
	[IND_EXT_ID] [nvarchar](max) NULL,
	[OWNER] [nvarchar](max) NULL,
	[ACT_STATUS] [nvarchar](max) NULL,
	[WKP_NAME] [nvarchar](max) NULL,
	[WKP_TEL] [nvarchar](max) NULL,
	[ZIP] [nvarchar](max) NULL,
	[PROVINCE] [nvarchar](max) NULL,
	[CITY] [nvarchar](max) NULL,
	[DONG] [nvarchar](max) NULL,
	[STREET] [nvarchar](max) NULL,
	[FULL_ADDR] [nvarchar](max) NULL,
	[IND_SP] [nvarchar](max) NULL,
	[TITLE] [nvarchar](max) NULL,
	[IND_LASTNAME] [nvarchar](max) NULL,
	[IND_FIRSTNAME] [nvarchar](max) NULL,
	[IND_FULL_NAME] [nvarchar](max) NULL,
	[EMAIL] [nvarchar](max) NULL,
	[MOBILE] [nvarchar](max) NULL,
	[CONSENT_SOURCE] [nvarchar](max) NULL,
	[CONSENT_SUB_SOURCE] [nvarchar](max) NULL,
	[EXTRACT_DATE] [datetime] NULL,
	[COUNTRY_CD] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL,
	[creater] [nvarchar](max) NULL,
	[modifieddate] [datetime] NOT NULL,
	[status] [int] NOT NULL,
	[pcmsid] [nvarchar](max) NULL,
	[channelId] [int] NULL,
	[SOURCE] [nvarchar](max) NULL,
	[WKP_ID] [nvarchar](max) NULL,
	[WKP_EXT_ID] [nvarchar](max) NULL,
	[OneKey] [nvarchar](max) NULL,
	[NucleusKey] [nvarchar](max) NULL,
	[LINK_RESERVATION] [nvarchar](max) NULL,
	[LINK_PHONE] [nvarchar](max) NULL,
	[LINK_ALERTED] [bit] NOT NULL,
	[SENDCHANEL] [int] NOT NULL,
 CONSTRAINT [PK_dbo.Privacies] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PrivacyLogs]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PrivacyLogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[changes] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL,
	[creater] [nvarchar](max) NULL,
	[privacy_ID] [int] NULL,
 CONSTRAINT [PK_dbo.PrivacyLogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Settings]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Settings](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[type] [int] NOT NULL,
	[name] [nvarchar](max) NULL,
	[value] [nvarchar](max) NULL,
 CONSTRAINT [PK_dbo.Settings] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Userlogs]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Userlogs](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](max) NULL,
	[useremail] [nvarchar](max) NULL,
	[ip] [nvarchar](max) NULL,
	[url] [nvarchar](max) NULL,
	[reqtype] [nvarchar](max) NULL,
	[parameters] [nvarchar](max) NULL,
	[createdate] [datetime] NOT NULL,
 CONSTRAINT [PK_dbo.Userlogs] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2016-10-27 오후 2:25:25 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[ID] [int] IDENTITY(1,1) NOT NULL,
	[username] [nvarchar](max) NULL,
	[roletype] [int] NOT NULL,
 CONSTRAINT [PK_dbo.UserRoles] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
ALTER TABLE [dbo].[Batches] ADD  DEFAULT ((0)) FOR [bound]
GO
ALTER TABLE [dbo].[Channels] ADD  DEFAULT ((0)) FOR [Instantrun]
GO
ALTER TABLE [dbo].[Consents] ADD  DEFAULT ((0)) FOR [CONSENT_USE]
GO
ALTER TABLE [dbo].[Consents] ADD  DEFAULT ((0)) FOR [CONSENT_TRUST]
GO
ALTER TABLE [dbo].[Consents] ADD  DEFAULT ((0)) FOR [CONSENT_ABROAD]
GO
ALTER TABLE [dbo].[Consents] ADD  DEFAULT ((0)) FOR [CONSENT_SIGN]
GO
ALTER TABLE [dbo].[Privacies] ADD  DEFAULT ((0)) FOR [LINK_ALERTED]
GO
ALTER TABLE [dbo].[Privacies] ADD  DEFAULT ((0)) FOR [SENDCHANEL]
GO
ALTER TABLE [dbo].[Approvals]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Approvals_dbo.Privacies_privacyId] FOREIGN KEY([privacyId])
REFERENCES [dbo].[Privacies] ([ID])
GO
ALTER TABLE [dbo].[Approvals] CHECK CONSTRAINT [FK_dbo.Approvals_dbo.Privacies_privacyId]
GO
ALTER TABLE [dbo].[Collections]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Collections_dbo.Channels_channelId] FOREIGN KEY([channelId])
REFERENCES [dbo].[Channels] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Collections] CHECK CONSTRAINT [FK_dbo.Collections_dbo.Channels_channelId]
GO
ALTER TABLE [dbo].[Consents]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Consents_dbo.Privacies_privacy_ID] FOREIGN KEY([privacy_ID])
REFERENCES [dbo].[Privacies] ([ID])
GO
ALTER TABLE [dbo].[Consents] CHECK CONSTRAINT [FK_dbo.Consents_dbo.Privacies_privacy_ID]
GO
ALTER TABLE [dbo].[N360File]  WITH CHECK ADD  CONSTRAINT [FK_dbo.N360File_dbo.Collections_collectionId] FOREIGN KEY([collectionId])
REFERENCES [dbo].[Collections] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[N360File] CHECK CONSTRAINT [FK_dbo.N360File_dbo.Collections_collectionId]
GO
ALTER TABLE [dbo].[PforceRXFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PforceRXFiles_dbo.Collections_CollectionId] FOREIGN KEY([CollectionId])
REFERENCES [dbo].[Collections] ([ID])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PforceRXFiles] CHECK CONSTRAINT [FK_dbo.PforceRXFiles_dbo.Collections_CollectionId]
GO
ALTER TABLE [dbo].[PforceRXFiles]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PforceRXFiles_dbo.Privacies_privacyId] FOREIGN KEY([privacyId])
REFERENCES [dbo].[Privacies] ([ID])
GO
ALTER TABLE [dbo].[PforceRXFiles] CHECK CONSTRAINT [FK_dbo.PforceRXFiles_dbo.Privacies_privacyId]
GO
ALTER TABLE [dbo].[Privacies]  WITH CHECK ADD  CONSTRAINT [FK_dbo.Privacies_dbo.Channels_channelId] FOREIGN KEY([channelId])
REFERENCES [dbo].[Channels] ([ID])
GO
ALTER TABLE [dbo].[Privacies] CHECK CONSTRAINT [FK_dbo.Privacies_dbo.Channels_channelId]
GO
ALTER TABLE [dbo].[PrivacyLogs]  WITH CHECK ADD  CONSTRAINT [FK_dbo.PrivacyLogs_dbo.Privacies_privacy_ID] FOREIGN KEY([privacy_ID])
REFERENCES [dbo].[Privacies] ([ID])
GO
ALTER TABLE [dbo].[PrivacyLogs] CHECK CONSTRAINT [FK_dbo.PrivacyLogs_dbo.Privacies_privacy_ID]
GO

INSERT INTO [dbo].[Channels] ( [name] ,[bound] ,[type] ,[athour] ,[usage] ,[modifieddate] ,[host] ,[account] ,[pwd] ,[path] ,[action] ,[Instantrun] ,[exportpath])
     VALUES ('N360' , 0, 0 , 2, 1,NOW, NULL, NULL, NULL, NULL,0,0, NULL);
GO	 
INSERT INTO [dbo].[Channels] ( [name] ,[bound] ,[type] ,[athour] ,[usage] ,[modifieddate] ,[host] ,[account] ,[pwd] ,[path] ,[action] ,[Instantrun] ,[exportpath])
     VALUES ('N360' , 1, 0 , 2, 1,NOW, NULL, NULL, NULL, NULL,0,0, NULL);
GO

INSERT [dbo].[Settings] ([type], [name], [value]) VALUES (0, N'승인 발생시 알림 이메일', N'Choiy28;')
INSERT [dbo].[Settings] ([type], [name], [value]) VALUES (1, N'동의 유효기간(년)', N'1')
INSERT [dbo].[Settings] ([type], [name], [value]) VALUES (2, N'동의 만료 알림(일)', N'14')
INSERT [dbo].[Settings] ([type], [name], [value]) VALUES (3, N'화이자링크 승인시 알림', N'Choiy28;')
GO
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Kimd53', 0)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Parkm13', 0)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Parkm13', 3)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Kimd53', 3)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Hongm04', 3)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Seos02', 3)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Hongm04', 0)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Seos02', 0)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Choiy28', 0)
INSERT [dbo].[UserRoles] ([username], [roletype]) VALUES (N'Choiy28', 3)
GO