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
            console.error("Error sending POST request:", error);
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
                <button className="btn btn-primary m-1">Don't ask</button>
                <button className="btn btn-primary m-1" onClick={() => handleButtonClick(lastInterval - (lastInterval / 2))}>
                    Custom input by Drop down list
                </button>
                <button className="btn btn-primary m-1" onClick={() => handleButtonClick(lastInterval - (lastInterval / 2))}>
                    Custom input by calendar
                </button>
            </div>
            {answerMode === "SystemAnswer" && <SystemAnswer systemAnswer={systemAnswer} />}
            {answerMode === "SideBySide" && <SideBySide systemAnswer={systemAnswer} userAnswer={userAnswer} />}
            {answerMode === "UpDownAnswer" && <UpDownAnswer systemAnswer={systemAnswer} userAnswer={userAnswer} />}
        </>
    );
}
