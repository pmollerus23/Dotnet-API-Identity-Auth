using System;

namespace WebApplication.Exceptions;

public class FriendshipAlreadyExistsException : Exception
{
    public FriendshipAlreadyExistsException(){ }

    public FriendshipAlreadyExistsException(string message)
        : base(message)
    {
    }
    public FriendshipAlreadyExistsException(string message, Exception inner)
        : base(message, inner)
    {
    }
}

