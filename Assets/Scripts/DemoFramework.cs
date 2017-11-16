﻿using UnityEngine;
using Assets.Scripts.QuadTree; 


public class DemoFramework : MonoBehaviour {

    //摄像机对象
    public GameObject cameraGo ; 
    //地形对象
    public GameObject terrainGo;

    //顶点间的距离
    public Vector3 vertexScale; 

    //高度图的边长,也就是结点的个数
    public int heightSize;

    //是否从高度图读取高度信息
    //True从文件读取
    //False动态生成
    public bool isLoadHeightDataFromFile;
    public string heightFileName;

    public bool isGenerateHeightDataRuntime;
    public int iterations;
    [Range(0,255)]
    public int minHeightValue;
    [Range(0, 65536)]
    public int  maxHeightValue;
    [Range(0,0.9f)]
    public float filter;


    #region 地图Tile
    public Texture2D detailTexture;

    [Range(1, 2048)]
    public int terrainTextureSize = 256; 
    public Texture2D[] tiles; 

    #endregion



    private CQuadTreeTerrain mQuadTreeTerrain; 
   
    //1、读取高度图，
    //2、设置顶点间距离，
    //3、读取纹理
    //4、设置光照阴影
	void Start ()
    {
        mQuadTreeTerrain = new CQuadTreeTerrain();

        //制造高度图
        mQuadTreeTerrain.MakeTerrainFault(heightSize,iterations,(ushort)minHeightValue, (ushort)maxHeightValue,filter);
        mQuadTreeTerrain.SetHeightScale(vertexScale.y); 

        //设置对应的纹理块
        AddTile(enTileTypes.lowest_tile);
        AddTile(enTileTypes.low_tile);
        AddTile(enTileTypes.high_tile);
        AddTile(enTileTypes.highest_tile);
        mQuadTreeTerrain.GenerateTextureMap((uint)terrainTextureSize,(ushort)maxHeightValue,(ushort)minHeightValue);
        ApplyTerrainTexture(mQuadTreeTerrain.TerrainTexture);


    }


    private void ApplyTerrainTexture(  Texture2D texture )
    {
        if( terrainGo != null )
        {
            MeshRenderer meshRender = terrainGo.GetComponent<MeshRenderer>(); 
            if( meshRender != null )
            {
                Shader terrainShader = Shader.Find("Terrain/QuadTree/TerrainRender");
                if (terrainShader != null)
                {
                    meshRender.material = new Material(terrainShader);
                    if (meshRender.material != null)
                    {
                        meshRender.material.SetTexture("_MainTex",texture) ;
                        if( detailTexture != null )
                        {
                            meshRender.material.SetTexture("_DetailTex", detailTexture);
                        }
                    }
                }
            }
        }
    }


 


    #region 地图块操作


    void AddTile( enTileTypes tileType )
    {
        int tileIdx = (int)tileType;
        if (tileIdx < tiles.Length
            && tiles[tileIdx] != null )
        {
            mQuadTreeTerrain.AddTile((enTileTypes)tileIdx, tiles[tileIdx]);
        }
    }


    #endregion


    // Update is called once per frame
    void Update ()
    {
        GL.wireframe = true;

        if( mQuadTreeTerrain != null )
        {
            RenderInWireframe wireframeCtrl = cameraGo.GetComponent<RenderInWireframe>(); 
            mQuadTreeTerrain.Render(terrainGo, vertexScale, wireframeCtrl != null ? wireframeCtrl.wireframeMode : false );      
        }

        GL.wireframe = false;   
    }

}
