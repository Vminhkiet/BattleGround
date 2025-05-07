using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.TutorialInfo.Scripts.Login
{
    public class LoginContext
    {
        private ILoginStrategy _loginStrategy;
        public void SetLoginStrategy(ILoginStrategy loginStrategy)
        {
            _loginStrategy = loginStrategy;
        }

        // Thực hiện đăng nhập
        public void ExecuteLogin()
        {
            if (_loginStrategy != null)
            {
                _loginStrategy.Login();  // Gọi phương thức Login của chiến lược hiện tại
            }
            else
            {
                Debug.Log("Login strategy not set!");
            }
        }
    }
}
