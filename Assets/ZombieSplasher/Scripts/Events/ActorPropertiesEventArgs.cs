﻿using UnityEngine;

public class ActorPropertiesEventArgs : System.EventArgs
{

    public GameObject Sender { get; set; }
    public ActorProperties ActorProperties;

    public ActorPropertiesEventArgs(GameObject sender, ActorProperties actorProperties)
    {
        Sender = sender;
        ActorProperties = actorProperties;
    }
}