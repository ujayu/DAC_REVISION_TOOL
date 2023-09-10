import React, { useState, useEffect } from 'react';
import Answer from './question/Answer';
import TextArea from './question/TextArea';
import { Link, useLocation, useParams } from 'react-router-dom';
import axios from './../../axios';
import { useSelector } from 'react-redux';

export default function Point() {
  const [time, setTime] = useState(0);
  const [window, setWindow] = useState("Question");
  const [pointData, setPointData] = useState(null);
  const [isLoading, setIsLoading] = useState(true);
  const [userAnswer, setUserAnswer] = useState("");
  const [error, setError] = useState(null);
  const { pointId } = useParams();
  const tokens = useSelector((state) => state.token);
  const location = useLocation();

  const fetchData = () => {
    axios.get(`point/${pointId}`, {
      headers: {
        Authorization: `Bearer ${tokens.accessToken}`
      }
    })
    .then(response => {
      setPointData(response.data);
      setIsLoading(false);
    })
    .catch(error => {
      setError(error);
      setIsLoading(false);
    });
  };

  useEffect(() => {
    setWindow('Question');
    fetchData(); // Fetch data when the component mounts

    const timer = setInterval(() => {
      setTime(prevTime => prevTime + 1);
    }, 1000);

    return () => clearInterval(timer);
  },  [location]);

  useEffect(() => {
    // Fetch data whenever the window variable changes
    if (window === "Answer") {
      fetchData();
    }
  }, [window]);

  if (isLoading) {
    return <div>Loading...</div>;
  }

  if (error) {
    return <div>Error: {error.message}</div>;
  }

  const showAnswer = () => {
    setWindow('Answer');
  };

  return (
    <>
        <div className="position-absolute bottom-0 end-0">
            <div>Timer: {time} seconds</div>
        </div>
        <div className="ms-4">
          <ol className="breadcrumb">
            <li className="breadcrumb-item"><Link to="/module/moduleId">Module</Link></li>
            <li className="breadcrumb-item active">Point</li>
          </ol>
        </div>
      <div className="row m-2">
            <div className="mb-3">
              <div className="border p-3  bg-primary" style={{ width: '100%'}}>
                {pointData.point1}
              </div>
            </div>
            {window === "Question" && <TextArea showAnswer={showAnswer} setUserAnswer={setUserAnswer} />}
            {window === "Answer" && <Answer systemAnswer={pointData.description} userAnswer={userAnswer} time={time} />}
      </div>
    </>
  );
};