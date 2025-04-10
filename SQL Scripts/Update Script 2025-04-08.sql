USE [GenAI]
GO

/****** Object:  Table [dbo].[GenAIHistory]    Script Date: 4/8/2025 11:39:44 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[GenAIHistory](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UniqueId] [uniqueidentifier] NOT NULL,
	[QuestionId] [uniqueidentifier] NOT NULL,
	[Question] [nvarchar](2000) NOT NULL,
	[UsecaseSpecificData] [nvarchar](4000) not null,
	[IsCorrect] [bit] NULL,
	[TicketOpened] [bit] NOT NULL,
	[Usecase] [tinyint] NOT NULL,
	[CreationDateTime] [datetime] NOT NULL,
	[CreatedBy] [bigint] NOT NULL,
 CONSTRAINT [PK_GenAIHistory] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[GenAIHistory]  WITH CHECK ADD  CONSTRAINT [FK_GenAIHistory_Users_Created] FOREIGN KEY([CreatedBy])
REFERENCES [dbo].[Users] ([Id])
GO

ALTER TABLE [dbo].[GenAIHistory] CHECK CONSTRAINT [FK_GenAIHistory_Users_Created]
GO


