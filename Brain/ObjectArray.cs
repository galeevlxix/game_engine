﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace game_2.Brain
{
    public class ObjectArray
    {
        private Dictionary<string, GameObj> dict;
        

        public ObjectArray()
        {
            dict = new Dictionary<string, GameObj>();
        }

        public ObjectArray(string name, GameObj gameObj)
        {
            dict = new Dictionary<string, GameObj>();
            Add(name, gameObj);
        }

        public ObjectArray(Dictionary<string, GameObj> dict)
        {
            this.dict = dict;
        }

        public void Add(string name, GameObj gameObj)
        {
            dict.Add(name, gameObj);
        }

        public void Remove(string name)
        {
            dict.Remove(name);
        }

        public void Clear()
        {
            dict.Clear();
        }

        public bool Contains(string name)
        {
            return dict.ContainsKey(name);
        }

        public int Count 
        { 
            get 
            { 
                return dict.Count; 
            } 
        }

        public void Draw()
        {
            foreach (GameObj gameObj in dict.Values)
            {
                gameObj.Draw();
            }
        }

        public void Reset()
        {
            foreach(GameObj gameObj in dict.Values)
            {
                gameObj.pipeline.Reset();
            }
        }

        public GameObj this [string name]
        {
            get
            {
                return dict[name];
            }
            set
            {
                dict[name] = value;
            }
        }

        public void Rotate(string name, float x, float y, float z)
        {
            dict[name].pipeline.Rotate(x, y, z);
        }

        public void Position(string name, float x, float y, float z)
        {
            dict[name].pipeline.Position(x, y, z);
        }

        public void Scale(string name, float x, float y, float z)
        {
            dict[name].pipeline.Scale(x, y, z);
        }

        public void ChangeWindowSize(int width, int height)
        {
            foreach (GameObj gameObj in dict.Values)
            {
                gameObj.pipeline.ChangeWindowSize((float)width, (float)height);
            }
        }
    }
}