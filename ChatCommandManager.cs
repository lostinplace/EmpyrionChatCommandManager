using Eleon.Modding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ChatCommandHandler = System.Action<Eleon.Modding.ChatInfo, Eleon.Modding.PString>;

namespace DebugMod
{
    public class ChatCommandManager
    {
        private Dictionary<string, Action<ChatInfo, PString>> chatActions;

        public ChatCommandManager(Dictionary<string, ChatCommandHandler> actions)
        {
            chatActions = actions;
        }

        public void HandleMessage(CmdId eventType, object data)
        {
            if (!(eventType == CmdId.Event_ChatMessage)) return;
            Handle_chat_message((ChatInfo)data);
        }

        public void Handle_chat_message(ChatInfo data)
        {
            if (data.type != 3) return;

            foreach (var item in chatActions)
            {
                var r = new Regex($"\\\\{item.Key}");
                var match = r.Match(data.msg);

                if (!match.Success) continue;

                var content = match.Groups.Count > 0 ? match.Groups[1].ToString() : "";
                item.Value.Invoke(data, new PString(content));
            }
        }
    }
}
