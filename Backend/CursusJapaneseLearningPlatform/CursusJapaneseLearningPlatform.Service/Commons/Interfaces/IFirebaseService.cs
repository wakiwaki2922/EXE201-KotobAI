using FirebaseAdmin.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CursusJapaneseLearningPlatform.Service.Commons.Interfaces;

public interface IFirebaseService
{
    Task<FirebaseToken> VerifyTokenAsync(string token);
}
