﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEngine.Random;

public class NonogramPuzzle
{
    private Sprite _dataMatrix;

    private NonogramGenerator _generator;

    private readonly string _serialNumber;

    public int[] GeneratedNumbers;

    public NonogramPuzzle(string serialNumber)
    {
        _serialNumber = serialNumber;
    }

    private bool[] GetFullClusters() => Reduce(false).Select(x => x == Color.black).ToArray();

    public bool[] GetPuzzleClusters() => Reduce(true).Select(x => x == Color.black).ToArray();

    private Color32[] Reduce(bool useForPuzzle)
    {

        var exclude = new[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 23, 35, 47, 59, 71, 83, 95, 107, 119, 131, 143, 142, 141, 140, 139, 138, 137, 136, 135, 134, 133, 132, 120, 108, 96, 84, 72, 60, 48, 36, 24, 12 };

        var colors = _dataMatrix.texture.GetPixels32();

        return (useForPuzzle ? Enumerable.Range(0, 144).Where(x => !exclude.Contains(x)) : Enumerable.Range(0, 144)).Select(x => colors[(11 - (x / 12)) * 12 + (x % 12)]).ToArray();
    }

    public void Generate(out List<string> horizClues, out List<string> vertClues, out bool[] valid, out List<List<string>> logged)
    {
        var q = new Queue<int[]>(Enumerable.Range(0, 20).Select(x => Enumerable.Range(0, 7).Select(_ => Range(0, 10)).ToArray()).ToList().Shuffle());


        
        while (q.Count > 0)
        {
            var nums = q.Dequeue();
            _dataMatrix = DataMatrixGenerator.GenerateDataMatrix(nums.Join(""));
            var encoder = new DataMatrixEncoder(GetPuzzleClusters(), GetFullClusters());
            var encoded = encoder.EncodeDataMatrix(_serialNumber);
            _generator = new NonogramGenerator(encoded);

            if (_generator.IsUnique())
            {
                horizClues = _generator.PrintedHorizClues;
                vertClues = _generator.PrintedVertClues;
                valid = encoded;
                GeneratedNumbers = nums;
                logged = encoder.Logged;
                return;
            }
        }
        

        throw new Exception("Cannot generator nonogram.");
    }
    
}
