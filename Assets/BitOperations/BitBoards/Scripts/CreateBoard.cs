using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;
using UnityEngine.Tilemaps;
using UnityEditor.IMGUI.Controls;
using System.ComponentModel;


public class CreateBoard : MonoBehaviour
{
     [Header("UI References")]
     [SerializeField]private GameObject[] tilePrefabs;
     [SerializeField] private GameObject housePrefab;
      [SerializeField] private GameObject treePrefab;
     [SerializeField] private TextMeshProUGUI score;
     GameObject[] tiles;
     long desertBoard = 0;
     long dirtBoard = 0;
     long treeBB = 0;
     long playerBB = 0;
   
    void Start()
    {
        tiles = new GameObject[64];
        for(int r=0; r<8; r++ ){
            for(int c=0; c<8;c++){
                int randomTile = UnityEngine.Random.Range(0,tilePrefabs.Length);
                Vector3 pos = new Vector3(c,0,r);
                 GameObject tile = Instantiate(tilePrefabs[randomTile], pos,Quaternion.identity); 

                 tile.name = tile.tag +"_" + r + "," + c; 
                 tiles[r*8+c] = tile;
                 tile.transform.parent = transform;
              
                 if(tile.tag == "Dirt"){
                    dirtBoard = SetCellState(dirtBoard,r,c);
                 }
                 if(tile.tag== "Desert"){
                    desertBoard = SetCellState(desertBoard,r,c);
                 }
                 
                 
            }
           
        }
       
        Debug.Log("Dirt Cell Count :" + CellCount(dirtBoard));
        InvokeRepeating("PlantTree",1,1);
    }
    void PlantTree(){
        int randomRow =UnityEngine.Random.Range(0,8);
        int randomCol =UnityEngine.Random.Range(0,8);
        if(GetCellState(dirtBoard & ~playerBB ,randomRow,randomCol))
        {
           GameObject tree = Instantiate(treePrefab);
           tree.transform.parent= tiles[randomRow*8+randomCol].transform;
           tree.transform.localPosition = Vector3.zero;
           treeBB = SetCellState(treeBB,randomRow,randomCol);
        }
    }
    void PrintBB(string name, long BB){
        Debug.Log(name + ":" + Convert.ToString(BB,2).PadLeft(64,'0'));
    }
    void PrintScore(){
        score.text = $"Score: {(CellCount(playerBB & dirtBoard)* 10) + (CellCount(playerBB & desertBoard)*2)} ";
    }
    long SetCellState(long bitBoard,int r, int c){
        long newBit  = 1L<< (r*8 + c);
        return (newBit  | bitBoard);
    }
    bool GetCellState(long bitBoard, int r, int c){
        long mask = 1L << (r*8+c);
        return ((bitBoard & mask)!=0);
    }
    int CellCount(long bitBoard){
        int count =0;
        long bb = bitBoard;
        while (bb>0){
            bb &= bb-1;
            count++;
        }
        return count;
    }
    void Update()
    {
        if(Input.GetMouseButton(0)){
           RaycastHit hit;
           var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
           if(Physics.Raycast(ray,out hit) ){
           
           int row =  (int)hit.collider.gameObject.transform.position.z;
           int col = (int)hit.collider.gameObject.transform.position.x;
           if(GetCellState((dirtBoard & ~treeBB) | desertBoard, row ,col)){
            GameObject house = Instantiate(housePrefab);
            house.transform.parent = hit.collider.gameObject.transform;
            house.transform.localPosition = Vector3.zero;
            
            playerBB=SetCellState(playerBB,row,col );
            PrintScore();
           }
           
           
           
        }
    }
    }
}