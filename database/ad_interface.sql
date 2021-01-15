USE [ad_interface]
GO
/****** Object:  Table [dbo].[t_errors]    Script Date: 05.10.2020 08:38:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[t_errors](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[samaccountname_a] [nvarchar](50) NOT NULL,
	[samaccountname_b] [nvarchar](50) NOT NULL,
	[field_a] [nvarchar](50) NOT NULL,
	[field_b] [nvarchar](50) NOT NULL,
	[value_a] [nvarchar](50) NOT NULL,
	[expected_value_a] [nvarchar](50) NOT NULL,
	[value_b] [nvarchar](50) NOT NULL,
	[save_date] [datetime] NOT NULL,
	[validate] [bit] NOT NULL,
	[validation_date] [datetime] NULL,
	[validator] [nvarchar](50) NULL,
	[fixed] [bit] NOT NULL,
	[fix_date] [datetime] NULL,
	[corrector] [nvarchar](50) NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[t_errors] ADD  CONSTRAINT [DF_t_errors_save_date]  DEFAULT (getdate()) FOR [save_date]
GO
ALTER TABLE [dbo].[t_errors] ADD  CONSTRAINT [DF_t_errors_validate]  DEFAULT ((0)) FOR [validate]
GO
ALTER TABLE [dbo].[t_errors] ADD  CONSTRAINT [DF_t_errors_fixed]  DEFAULT ((0)) FOR [fixed]
GO
