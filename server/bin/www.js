const express = require('express');
const app = express();
const http = require('http');
const server = http.createServer(app);
const io = require('socket.io')(server);
const date = require('date-utils');

//player name : P1, P2
let ready = [false, false];
let score = [0, 0];
let pCount = 0;
let playing = false;

//about time
let timer = 0;
let timeMst = "";
let timeSst = "";

const gameEndTimer = 120;
const gameEndScore = 15;

let itemTable = [];
let itemTableEndIndex = 0;

const itemInterval = 3;

let hostIp = "";
let clientIp = "";

const addScoreNormal = 1;

const plusScoreAddition = 1;
let plusScoreActivation = false;
let plusScoreTime;
let plusScorePlayer = "";
const plusScoreInterval = 15; //15초

let player1BigGoalActivation = false;
let player2BigGoalActivation = false;
let player1SmallGoalActivation = false;
let player2SmallGoalActivation = false;
let bigGoalPlayer = "";
let smallGoalPlayer = "";
const goalItemInterval = 15;
let player1GoalItemTime;
let player2GoalItemTime;

let penaltyKickPlayer = "";

const feverTimeInterval = 15;
let feverTimeActivation = false;
let feverTimeScoreAddition = 2;

let onceTimer = null;

function initGame() {

    ready = [false, false];
    score = [0, 0];
    pCount = 0;
    playing = false;

    timer = 0;
    timeMst = "";
    timeSst = "";


    itemTable = [];
    itemTableEndIndex = 0;

    hostIp = "";
    clientIp = "";

    plusScoreActivation = false;
    plusScoreTime = 0;
    plusScorePlayer = "";

    player1BigGoalActivation = false;
    player2BigGoalActivation = false;
    player1SmallGoalActivation = false;
    player2SmallGoalActivation = false;
    bigGoalPlayer = "";
    smallGoalPlayer = "";
    player1GoalItemTime = 0;
    player2GoalItemTime = 0;

    penaltyKickPlayer = "";

    feverTimeActivation = false;
    feverTimeScoreAddition = 2;

    onceTimer = null;
}

function makeItemTable() {
    //아이템 확률 등이 적혀있는 list를 초기화
    addItemRandomTable(22, "DoubleScore");
    addItemRandomTable(18, "BigGoal");
    addItemRandomTable(18, "SmallGoal");
    addItemRandomTable(12, "PenaltyKick");
}

function addItemRandomTable(percentage, itemName) {
    //아이템 확률을 구해서 jsonList에 넣음
    let itemTableJson = {
        percentageStart: 0,
        percentageEnd: 100,
        itemName: "ITEMNAME"
    };
    itemTableJson.percentageStart = itemTableEndIndex;
    itemTableEndIndex += percentage;
    itemTableJson.percentageEnd = itemTableEndIndex;
    itemTableJson.itemName = itemName;
    itemTable.push(itemTableJson);
}

function toTimeString(timer) {
    let m = parseInt(timer / 60);
    let s = timer % 60;
    let mst = m < 10 ? "0" + m : m;
    let sst = s < 10 ? "0" + s : s;
    timeMst = mst;
    timeSst = sst;
}

function gameStart(io) {
    io.sockets.emit("scoreUpEmit", {p1Score: 0 + "", p2Score: 0 + ""});
    io.sockets.emit("gameStart", {hostIp: hostIp, clientIp: clientIp});
    console.log("GAME START!");
    playing = true;

    score[0] = 0;
    score[1] = 0;

    //다음게임을 위한 초기화
    ready[0] = false;
    ready[1] = false;
    pCount = 0;

    //timer setting
    onceTimer = setInterval(timerSetting, 1000);
}

function gameEnd(io, onceTimer) {
    playing = false;
    feverTimeActivation = false;
    clearInterval(onceTimer);
    timer = 0;

    let winnerPlayer = "";
    if (score[0] > score[1]) {
        winnerPlayer = "P1";
    } else if (score[0] < score[1]) {
        winnerPlayer = "P2";
    } else {
        winnerPlayer = "DRAW";
    }

    io.sockets.emit("gameEndEmit", {winner: winnerPlayer});

    console.log("game End, winner : " + winnerPlayer);

    initGame();
}

function timerSetting() {
    timer++;
    toTimeString(timer);
    console.log(timeMst + " : " + timeSst);
    io.sockets.emit("timerEmit", {timeMst: timeMst, timeSst: timeSst});

    if (timer % itemInterval === 0) {
        let randomItemNum = Math.floor(Math.random() * 100);
        console.log("random item num : " + randomItemNum);
        let itemJson = {itemName: ""};

        itemTable.some(itemPercentageJson => {
            let isValidPercentage = randomItemNum >= itemPercentageJson.percentageStart && randomItemNum < itemPercentageJson.percentageEnd;
            if (isValidPercentage) {
                itemJson.itemName = itemPercentageJson.itemName;
                console.log("item " + itemPercentageJson.itemName + " is emit");
                io.sockets.emit("itemEmit", itemJson);
                return isValidPercentage;
            }
        });

    }

    // 두배 점수 아이템 효과
    if (plusScoreActivation) {
        if (timer > plusScoreTime + plusScoreInterval) {
            //20초가 지나면 효과 끝
            plusScoreActivation = false;
            console.log("endItem double score");
            io.sockets.emit("endItemEmit", {itemName: "DoubleScore"});
        }
    }

    if (player1BigGoalActivation) {
        if (timer > player1GoalItemTime + goalItemInterval) {
            player1BigGoalActivation = false;
            console.log("endItem player1 big goal");
            io.sockets.emit("endItemEmit", {itemName: "BigGoal", player: "P1"});
        }
    }
    if (player1SmallGoalActivation) {
        if (timer > player1GoalItemTime + goalItemInterval) {
            player1SmallGoalActivation = false;
            console.log("endItem player1 small goal");
            io.sockets.emit("endItemEmit", {itemName: "SmallGoal", player: "P1"});
        }
    }

    if (player2BigGoalActivation) {
        if (timer > player2GoalItemTime + goalItemInterval) {
            player2BigGoalActivation = false;
            console.log("endItem player2 big goal");
            io.sockets.emit("endItemEmit", {itemName: "BigGoal", player: "P2"});
        }
    }
    if (player2SmallGoalActivation) {
        if (timer > player2GoalItemTime + goalItemInterval) {
            player2SmallGoalActivation = false;
            console.log("endItem player2 small goal");
            io.sockets.emit("endItemEmit", {itemName: "SmallGoal", player: "P2"});
        }
    }

    //Fever time
    if (!feverTimeActivation) {
        if (timer >= (gameEndTimer - feverTimeInterval)) {
            feverTimeActivation = true;
            io.sockets.emit("feverTimeEmit", {});
            console.log("Fever time! U can get double score!")
        }
    }

    if (timer >= gameEndTimer && playing === true) {
        gameEnd(io, onceTimer);
    }
}

function hockey(io) {

    //socket.emit 현재 연결되어 있는 클라이언트 소켓에
    //socket.broadcast.emit 나를 제외한 클라이언트 소켓에
    //io.sockets.emit 나를 포함한 모든 클라이언트 소켓에

    io.on('connection', (socket) => {
        console.log("socket server : connection on");

        socket.on("userReady", (data) => {
            console.log("userReady ");
            if (pCount === 0) {
                socket.emit("userReadyOn", {player: "P1"});
                pCount++;

                ready[0] = true;
                hostIp = data.ip;
                console.log("P1 IS READY, IP : " + hostIp);
            } else if (pCount === 1) {
                socket.emit("userReadyOn", {player: "P2"});
                pCount++;

                ready[1] = true;
                clientIp = data.ip;
                console.log("P2 IS READY, IP : " + clientIp);
            }
            //두명 레디 신호시 게임 시작
            if (ready[0] && ready[1] && !playing) {
            //if (ready[0] && !playing) {

                gameStart(io);
            }
        });

        socket.on("handPosition", (data) => {
            //handPosition을 p2 가 p1에게 보낸다
            if (data.player === "P2") {
                // console.log("HAND_POSITION(P2): " + data.player + "(" + data.x + "," + data.y + "," + data.z + ")");
                //socket.broadcast.emit("handPositionEmit", {x: data.x, y: data.y, z: data.z});
            }
        });

        socket.on("eatItem", (data) => {
            console.log("eat item , item name : " + data.itemName);
            switch (data.itemName) {
                case "DoubleScore":
                    plusScoreTime = timer;
                    plusScoreActivation = true;
                    plusScorePlayer = data.player;
                    console.log("DoubleScore, item player : " + data.player);
                    io.sockets.emit("eatItemEmit", {itemName: "DoubleScore", player: plusScorePlayer});
                    break;
                case "BigGoal":
                    bigGoalPlayer = data.player;
                    console.log("BigGoal, item player : " + data.player);
                    if (bigGoalPlayer === "P1") {
                        player1GoalItemTime = timer;
                        player1BigGoalActivation = true;
                    } else {
                        player2GoalItemTime = timer;
                        player2BigGoalActivation = true;
                    }
                    io.sockets.emit("eatItemEmit", {itemName: "BigGoal", player: bigGoalPlayer});
                    break;
                case "SmallGoal":
                    smallGoalPlayer = data.player;
                    console.log("SmallGoal, item player : " + data.player);
                    if (smallGoalPlayer === "P1") {
                        player1GoalItemTime = timer;
                        player1SmallGoalActivation = true;
                    } else {
                        player2GoalItemTime = timer;
                        player2SmallGoalActivation = true;
                    }
                    io.sockets.emit("eatItemEmit", {itemName: "SmallGoal", player: smallGoalPlayer});
                    break;
                case "PenaltyKick":
                    penaltyKickPlayer = data.player;
                    console.log("PenaltyKick, item player: " + penaltyKickPlayer);
                    io.sockets.emit("eatItemEmit", {itemName: "PenaltyKick", player: penaltyKickPlayer});
                    break;
            }
        });

        socket.on("scoreUp", (data) => {
            console.log("SCORE_UP data player : " + data.player);
            let addScore = addScoreNormal;
            let addScoreIndex = 0;
            if (data.player === "P1") {
                addScoreIndex = 0;
            } else {
                addScoreIndex = 1;
            }
            if(plusScoreActivation) {
                if((addScoreIndex === 0 && plusScorePlayer === "P1") || (addScoreIndex === 1 && plusScorePlayer === "P2")) {
                    addScore += plusScoreAddition;
                }
            }
            if (feverTimeActivation) {
                addScore += feverTimeScoreAddition;
            }
            score[addScoreIndex] += addScore;

            console.log("SCORE_UP data :  " + score[0] + ":" + score[1]);

            io.sockets.emit("scoreUpEmit", {p1Score: score[0] + "", p2Score: score[1] + ""});

            if (score[0] >= gameEndScore || score[1] >= gameEndScore) {
                gameEnd(io, onceTimer);
            }
        });

        socket.on("gameRestart", (data) => {
            console.log("gameRestart");
            if(playing === false) {
                io.sockets.emit("gameRestartEmit", {playing : true});
                gameStart(io);
            }
        });
    });
}

initGame();

makeItemTable();

itemTable.forEach(itemTableJson => {
    console.dir(itemTableJson);
});

hockey(io);

server.listen(3000, () => {
    console.log("SocketIO server is listening on 3000");
});