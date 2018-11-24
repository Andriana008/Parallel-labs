#define N 1000
#include <ctime>
#include <stdio.h>
#include <stdlib.h>
#include <mpi.h>

using namespace std;

MPI_Status status;

double matA[N][N], matB[N][N], matC[N][N], matD[N][N];

void fillMatrices()
{
    for (int i = 0; i < N; i++)
    {
        for (int j = 0; j < N; j++)
        {
            matA[i][j] = 1.0;
            matB[i][j] = 2.0;
        }
    }
}

void SumMatrices()
{
    for (int i = 0; i < N-1; i++)
    {
        for (int j = 0; j < N-1; j++)
        {
            matD[i][j]=matA[i][j]+matB[i][j];
        }
    }
}

int main(int argc,char * argv[]) {
    
    srand(time_t(NULL));
    clock_t start;
    start = clock();
    SumMatrices();
    //printf( "\ntime taken by one thread:%lf\n",(double)(clock() - start) / CLOCKS_PER_SEC);
    //double first=(double)(clock() - start) / CLOCKS_PER_SEC;

    

    int rank,size;
    int i,j;
    
    MPI_Request request[10];
    MPI_Status status;
    
    MPI_Init(&argc, &argv);
    MPI_Comm_size(MPI_COMM_WORLD, &size);
    MPI_Comm_rank(MPI_COMM_WORLD,&rank);
    
    if(rank==0){
        
        double starttime,endtime;
        int rows=N;
        int columns=N;
        
        starttime = MPI_Wtime();
        
        /* add in part of rank 0 */
        int eachRow = rows/size;
        int ModRow = rows%size;
        
        for (i=1; i<size; i++) {
            MPI_Isend(&columns, 1, MPI_INT, i, 112, MPI_COMM_WORLD, &request[0]);
            MPI_Isend(&eachRow, 1, MPI_FLOAT, i, 111, MPI_COMM_WORLD,&request[1]);
            MPI_Isend(&matA[(eachRow*i)+ModRow][0], eachRow*columns, MPI_FLOAT, i, 113, MPI_COMM_WORLD,&request[2]);
            MPI_Isend(&matB[(eachRow*i)+ModRow][0], eachRow*columns, MPI_FLOAT, i, 114, MPI_COMM_WORLD,&request[3]);
            
        }
        
        for (i=0; i<eachRow+ModRow; i++) {
            for (j=0; j<columns; j++) {
                matC[i][j] = matA[i][j]+matB[i][j];
            }
        }
        
        
        for (i=1; i<size; i++) {
            MPI_Irecv(&matC[(eachRow*i)+ModRow][0],  eachRow*columns, MPI_FLOAT, i, 115, MPI_COMM_WORLD,&request[4]);
            MPI_Wait(&request[4], &status);
        }
        endtime = MPI_Wtime();
         
        printf("Total time : %lf\n",endtime-starttime);
//
//        double accel =  (endtime-starttime)/ first;
//        printf("Acceleration : %f\n", accel);
//
//        double efficeincy = accel/2;
//        printf("Efficiency : %f\n", efficeincy);
        
    }else{
        
        int eachRow ;
        int column ;
        
        MPI_Irecv(&column, 1, MPI_INT, 0, 112, MPI_COMM_WORLD, &request[5]);
        MPI_Irecv(&eachRow, 1, MPI_INT, 0, 111, MPI_COMM_WORLD,&request[6]);
        
        MPI_Wait(&request[5], &status);
        MPI_Wait(&request[6], &status);
        
       
        
        int i,j;
        
        MPI_Irecv(&matA[0][0],eachRow*column, MPI_FLOAT, 0, 113, MPI_COMM_WORLD,&request[7]);
        MPI_Irecv(&matB[0][0],eachRow*column, MPI_FLOAT, 0, 114, MPI_COMM_WORLD, &request[8]);
        
        MPI_Wait(&request[7], &status);
        MPI_Wait(&request[8], &status);
        
        for (i=0; i<eachRow; i++) {
            for (j=0; j<column; j++) {
                matC[i][j] = matA[i][j] + matB[i][j];
            }
        }
        
        MPI_Isend(&matC[0][0], eachRow*column, MPI_FLOAT, 0, 115, MPI_COMM_WORLD,&request[9]);
        MPI_Wait(&request[9], &status);
        
        
    }
    
    MPI_Finalize();
    
    
}
