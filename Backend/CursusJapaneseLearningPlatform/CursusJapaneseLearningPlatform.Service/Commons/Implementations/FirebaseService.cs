using CursusJapaneseLearningPlatform.Service.Commons.Exceptions;
using FirebaseAdmin.Auth;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CursusJapaneseLearningPlatform.Service.Commons.Interfaces;

namespace CursusJapaneseLearningPlatform.Service.Commons.Implementations;

public class FirebaseService : IFirebaseService
{
    private readonly FirebaseAuth _firebaseAuth;

    public FirebaseService()
    {
        if (FirebaseApp.DefaultInstance == null)
        {
            FirebaseApp.Create(new AppOptions
            {
                Credential = GoogleCredential.FromFile(Path.Combine(Directory.GetCurrentDirectory(), "firebase-config.json"))
            });
        }
        _firebaseAuth = FirebaseAuth.DefaultInstance;
    }

    public async Task<FirebaseToken> VerifyTokenAsync(string token)
    {
        try
        {
            return await _firebaseAuth.VerifyIdTokenAsync(token);
        }
        catch (FirebaseAuthException ex)
        {
            throw new CustomException(
                StatusCodes.Status401Unauthorized,
                ResponseCodeConstants.UNAUTHORIZED,
                "Invalid Firebase token"
            );
        }
    }
}
