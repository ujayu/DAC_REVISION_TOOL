import React, { useEffect, useState } from 'react';
import { useSelector } from 'react-redux';
import { useNavigate, useParams } from 'react-router-dom';
import axios from './../../../axios';
import SystemAnswer from './SystemAnswer';
import SideBySide from './SideBySide';
import UpDownAnswer from './UpDownAnswer';
import './Answer.css';

export default function Answer({ systemAnswer, userAnswer, time }) {
    const answerMode = useSelector((state) => state.settings.answerMode);
    const tokens = useSelector((state) => state.token);
    const { pointId } = useParams();
    const navigate = useNavigate();

    const [lastInterval, setLastInterval] = useState(0);
    const [minuteHourDay, setMinuteHourDay] = useState("");
    const [inputMHD, setInputMHD] = useState("Day");
    const [inputTime, setInputTime] = useState(0);

    useEffect(() => {
        fetchLastInterval();
    }, [pointId]);

    async function fetchLastInterval() {
        try {
            const response = await axios.get(`/Main/lastInterval/${pointId}`, {
                headers: {
                    Authorization: `Bearer ${tokens.accessToken}`
                }
            });

            const intervalInMinutes = response.data.intervalInMinutes;

            if (intervalInMinutes >= 1440) {
                setMinuteHourDay(" Day");
                setLastInterval(Math.floor(intervalInMinutes / 1440));
            } else if (intervalInMinutes >= 60) {
                setMinuteHourDay(" Hour");
                setLastInterval(Math.floor(intervalInMinutes / 60));
            } else {
                setMinuteHourDay(" Minutes");
                setLastInterval(intervalInMinutes);
            }
        } catch (error) {
            console.error("Error fetching last interval:", error);
        }
    }

    async function handleButtonClick(timeInMinutes) {
        try {
            // Calculate the next time by adding timeInMinutes to the current time

    
            const response = await axios.post(
                `/Main/nextAsk/${pointId}`,
                { NextTime: timeInMinutes}, // Convert to ISO string
                {
                    headers: {
                        Authorization: `Bearer ${tokens.accessToken}`
                    }
                }
            );

            console.log(response.data);

            navigate(`/point/${response.data.pointId}`);
        } catch (error) {
            if(error.response.data  === "No eligible points found."){
                navigate("/revisionComplete");
            }
            console.error("Error sending POST request:", error);
        }
    }

    async function dontAsk() {
        try {
          const response = await axios.put(`/Main/pointInRevision/${pointId}`, null, {
            headers: {
              Authorization: `Bearer ${tokens.accessToken}`,
            },
          });
          handleButtonClick(0);
        } catch (error) {
          console.error('Error updating point status:', error);
        }
      }

    return (
        <>
            <div className="text-center">
                {lastInterval === 0 && (
                    <button className="btn btn-primary m-1" onClick={() => handleButtonClick(60)}>
                        1 Hour
                    </button>
                )}
                {lastInterval !== 0 &&
                    [1, 0.5, 2, 2.5, 3].map((multiplier) => (
                        <button
                            key={multiplier}
                            className="btn btn-primary m-1"
                            onClick={() => handleButtonClick(lastInterval * multiplier)}
                        >
                            {lastInterval * multiplier + " " + minuteHourDay}
                        </button>
                    ))}
                <button className="btn btn-primary m-1" onClick={dontAsk}>Don't ask</button>
                <div class="input-group  btn-primary mb-3">
                    <select
                    class="form-select" id="dropdown-addon"
                    aria-label="Example dropdown with text addon"
                    aria-describedby="button-addon1"
                    onChange={(e)=>{setInputMHD(e.target.value)}}>
                        <option value="Minute">Minute</option>
                        <option value="Hour" >Hour</option>
                        <option value="Day" selected>Day</option>
                    </select>
                    <input type="text" class="form-control" placeholder="" onBlur={(e)=>{setInputTime(e.target.value)}} aria-label="Example text with dropdown addon" aria-describedby="button-addon1"/>
                <button className="btn btn-primary m-1" onClick={() => {
                    if(inputMHD === "Day"){
                        handleButtonClick(inputTime * 60 * 24 );
                    }else if( inputMHD === "Hour"){
                        handleButtonClick(inputTime * 60  );
                    }else{
                        handleButtonClick(inputMHD);
                    }
                }}>
                    Ask
                </button>
                </div>

            </div>
            {answerMode === "SystemAnswer" && <SystemAnswer systemAnswer={systemAnswer} />}
            {answerMode === "SideBySide" && <SideBySide systemAnswer={systemAnswer} userAnswer={userAnswer} />}
            {answerMode === "UpDownAnswer" && <UpDownAnswer systemAnswer={systemAnswer} userAnswer={userAnswer} />}
        </>
    );
}
