export default function TextArea({showAnswer, setUserAnswer}){
    function sendAnswer(e){
        setUserAnswer(e.target.value);
    }

    return(
        <div>
            <div className="d-flex justify-content-evenly">
                <button className="btn btn-primary" onClick={showAnswer}>Show Answer</button>
            </div>
            <div className="mt-3">
            <textarea className="form-control" rows="5" placeholder="Your answer..." style={{ width: '100%' }} onChange={sendAnswer}/>
            </div>
        </div>
    )
}