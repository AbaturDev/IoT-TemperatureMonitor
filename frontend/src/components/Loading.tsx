import "./Loading.css";

const Loading = ({ isLoading }: { isLoading: boolean }) => {
  if (isLoading)
    return (
      <div className="loading-container">
        <div className="loading-box">
          <div className="loading-dot a"></div>
          <div className="loading-dot b"></div>
          <div className="loading-dot c"></div>
          <div className="loading-dot d"></div>
        </div>
      </div>
    );
};

export default Loading;
