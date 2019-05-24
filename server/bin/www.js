const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const io = require('socket.io')(server);
const date = require('date-utils');

//player name : P1, P2
var ready = [false, false];
var score = [0, 0];
var pCount = 0;
var playing = false;

//about time
var timer = 0;
var timeMst = "";
var timeSst = "";

//about item
var item_timer = 9999;
var item_on = false;

const itemInterval = 3;
const numOfItem = 2;

let hostIp = "";
let clientIp = "";

const plusScoreNormal = 1;

const plusScoreDouble = 2;
let plusScoreActivation = false;
let plusScoreTime;
let plusScorePlayer = "";
const plusScoreInterval = 20; //20초


var timerSetting = () => {
    timer++;
    toTimeString(timer);
    console.log(timeMst + " : " + timeSst);
    io.sockets.emit("timerEmit", {timeMst: timeMst, timeSst: timeSst});

    if (timer % itemInterval === 0) {
        let randomItem = Math.floor(Math.random() * numOfItem);
        console.log("random item num : " + randomItem);
        let itemJson = {itemName: ""};
        switch (randomItem) {
            case 0:
                itemJson.itemName = "DoubleScore";
                break;
            case 1:
                itemJson.itemName = "BigGoal";
                break;
            case 2:
                break;
            case 3:
                break;
        }

        io.sockets.emit("itemEmit", itemJson);
    }

    // 두배 점수 아이템 효과
    if(plusScoreActivation) {
        if(timer > plusScoreTime + plusScoreInterval) {
            //20초가 지나면 효과 끝
            plusScoreActivation = false;
            io.sockets.emit("endItemEmit",  {itemName:"DoubleScore"});
        }
    }

    //Fever time
    if (timer >= (180 - 15)) {
        io.sockets.emit("feverTime");
        console.log("Fever time! U can get double score!")
    }

    if (timer >= 180 || playing == false) {
        io.sockets.emit("gameEndEmit");
        console.log("game End");
        playing = false;

        //인터발 정지 및 게임종료
        clearInterval(onceTimer);
        timer = 0;
    }

    //아이템 관련
    if (timer == 30 || timer == 60 || timer == 90 || timer == 120 || timer == 150) {
        io.sockets.emit("itemFloating");
        console.log("item floating");
    }
    if (!item_on && (timer == 40 || timer == 70 || timer == 100 || timer == 130 || timer == 160)) {
        io.sockets.emit("itemDelete");
        console.log("item is deleted , you cannot get!");
    }
    if (timer - item_timer >= 10) {
        io.sockets.emit("itemEnd");
        item_on = false;
    }
};

var onceTimer = null;

var toTimeString = (timer) => {
    var m = parseInt(timer / 60);
    var s = timer % 60;
    var mst = "";
    var sst = "";
    mst = "0" + m;
    if (s < 10) {
        sst = "0" + s;
    } else {
        sst = s;
    }
    timeMst = mst;
    timeSst = sst;
};


var hockey = (io) => {

    //socket.emit 현재 연결되어 있는 클라이언트 소켓에
    //socket.broadcast.emit 나를 제외한 클라이언트 소켓에
    //io.sockets.emit 나를 포함한 모든 클라이언트 소켓에

    io.on('connection', (socket) => {
        console.log("socket server : connection on");

        socket.on("userReady", (data) => {
            console.log("userReady ");
            if (pCount == 0) {
                socket.emit("userReadyOn", {player: "P1"});
                pCount++;

                ready[0] = true;
                hostIp = data.ip;
                console.log("P1 IS READY, IP : " + hostIp);
            } else if (pCount == 1) {
                socket.emit("userReadyOn", {player: "P2"});
                pCount++;

                ready[1] = true;
                clientIp = data.ip;
                console.log("P2 IS READY, IP : " + clientIp);
            }
            //두명 레디 신호시 게임 시작
            if (ready[0] && ready[1] && !playing) {
            //if (ready[0] && !playing) {
                io.sockets.emit("gameStart", {hostIp: hostIp, clientIp: clientIp});
                console.log("GAME START!");
                playing = true;

                //다음게임을 위한 초기화
                ready[0] = false;
                ready[1] = false;
                pCount = 0;

                //timer setting
                onceTimer = setInterval(timerSetting, 1000);
            }
        });

        //ballPosition P2P 통신 예정
        // socket.on("ballPosition", (data)=>{ //위치
        //     console.log("BALL_POSITION :" + "(" + data.x + "," + data.y + "," + data.z + ")");
        //     socket.broadcast.emit("ballPositionEmit", { x : data.x, y : data.y, z : data.z} ) ;
        // });

        socket.on("handPosition", (data) => {
            //handPosition을 p2 가 p1에게 보낸다
            if (data.player == "P2") {
                // console.log("HAND_POSITION(P2): " + data.player + "(" + data.x + "," + data.y + "," + data.z + ")");
                socket.broadcast.emit("handPositionEmit", {x: data.x, y: data.y, z: data.z});
            }
        });

        socket.on("eatItem", (data) => {
            console.log("eat item , item name : " + data.itemName);
            switch (data.itemName) {
                case "DoubleScore":
                    plusScoreTime = timer;
                    plusScoreActivation = true;
                    plusScorePlayer = data.player;
                    console.log("item player : " + data.player);
                    io.sockets.emit("eatItemEmit", {itemName:"DoubleScore", player:plusScorePlayer});
                    break;
            }
        });

        socket.on("scoreUp", (data) => {
            console.log("SCORE_UP data player : " + data.player);
            if (data.player === "P1") {
                if(plusScoreActivation && plusScorePlayer === "P1") {
                    console.log("player score up with item : P1");
                    score[0] += plusScoreDouble;
                } else {
                    score[0] += plusScoreNormal;
                }
            }
            if (data.player === "P2") {
                if(plusScoreActivation && plusScorePlayer === "P2") {
                    console.log("player score up with item : P2");
                    score[1] += plusScoreDouble;
                } else {
                    score[1] += plusScoreNormal;
                }
            }
            console.log("SCORE_UP data :  " + score[0] + ":" + score[1]);

            io.sockets.emit("scoreUpEmit", {p1Score: score[0] + "", p2Score: score[1] + ""});

            if (score[0] == 15 || score[1] == 15) {
                io.sockets.emit("gameEndEmit");
                playing = false;
            }
        });
        /*
                //about item

                socket.on("getItem", (data)=>{
                    item_on = true;
                    var randomItem = Math.random(4); //0,1,2,3

                    if(randomItem == 0){
                        io.sockets.emit("doubleScore");
                        item_timer = timer;
                        console.log("Item : double Score");
                        plus_score = 2;
                    }else if(randomItem == 1){
                        io.sockets.emit("bigGoal");
                        item_timer = timer;
                        console.log("Item : big Goal basket");
                    }else if(randomItem == 2){
                        io.sockets.emit("smallGoal");
                        item_timer = timer;
                        console.log("Item : small Goal basket");
                    }else if(randomItem == 3){
                        io.socket.emit("panaltyKick");
                        item_timer = timer;
                        console.log("Item : panalty Kick (reset a ball position on front of me)");
                    }
                });*/
    });
};

hockey(io);

server.listen(3000, () => {
    console.log("SocketIO server is listening on 3000");
});