# PRNChat

A modern chat platform built with WPF (.NET 9), featuring user authentication, friend management, group chat, and AI bot interaction. Supabase is used for authentication and chat history storage.

---

## Features

- **User Authentication**: Sign up and sign in securely using Supabase.
- **Friend Management**: Add and manage friends.
- **Group Chat**: Create and participate in group conversations.
- **AI Bot Chat**: Chat with an integrated AI bot.
- **Chat History**: All messages are stored and retrieved via Supabase.

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Visual Studio 2022 or later](https://visualstudio.microsoft.com/)
- [Supabase account](https://supabase.com/) with project and API keys

### Setup

1. **Clone the repository**
   git clone https://github.com/huynphgtr/PRN-Chat.git

2. **Configure Supabase:**
   - Add your Supabase URL and API keys to the `Client/Config` folder (e.g., `supabase.json` or as environment variables).

3. **Restore dependencies:**
   dotnet restore

4. **Build the project:**
    dotnet build
    dotnet run --project PRNChat

---

## Usage

- Launch the app.
- Sign up or sign in.
- Add friends, create groups, and start chatting.
- Interact with the AI bot for automated responses.

---

## Technologies Used

- **.NET 9 / WPF**: Modern desktop UI framework
- **Supabase**: Authentication and real-time database
- **MVVM Pattern**: Clean separation of UI and logic

---

## Branch Naming Rules
- Branch `main` is for production-ready code.
- Use `feature/` for new features 
  e.g: 
  `feature/sign-up`
  `feature/log-in`
  `feature/add-friend`
  `feature/group-chat`
  `feature/ai-bot`
   ...
- Use `bugfix/` for bug fixes 
  e.g: 
  `bugfix/fix-sign-up`
  `bugfix/fix-log-in`
  `bugfix/fix-add-friend`
  `bugfix/fix-group-chat`
  `bugfix/fix-ai-bot`
   ...
- Use `hotfix/` for urgent fixes that need immediate attention
  e.g: 
  `hotfix/urgent-fix-sign-up`
  `hotfix/urgent-fix-log-in`
  `hotfix/urgent-fix-add-friend`
  `hotfix/urgent-fix-group-chat`
  `hotfix/urgent-fix-ai-bot`
   ...
- Use `refactor/` for code refactoring 
  e.g: 
  `refactor/optimize-sign-up`
  `refactor/optimize-log-in`
  `refactor/optimize-add-friend`
  `refactor/optimize-group-chat`
  `refactor/optimize-ai-bot`
   ...
- Use `chore/` for maintenance tasks that do not affect the codebase
  e.g: 
  `chore/update-dependencies`
  `chore/clean-up-code`
  `chore/documentation-update`
   ...`

---

## Contributing

Contributions are welcome! Please open issues or submit pull requests for improvements or bug fixes.

---

## License

This project is licensed under the MIT License.



    
