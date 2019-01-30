using System;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Async_Await
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine(MyDoAsync().Result);
        }

        static async Task<int> DoAsync()
        {
            await Task.Delay(10000).ConfigureAwait(false);
            return 42;
        }

        static Task<int> MyDoAsync()
        {
            var stateMachine = new MyDoAsyncStateMachine();
            stateMachine.methodBuilder = new AsyncTaskMethodBuilder<int>();
            stateMachine.methodBuilder.Start(ref stateMachine);
            return stateMachine.methodBuilder.Task;
        }

        public struct MyDoAsyncStateMachine : IAsyncStateMachine
        {
            public AsyncTaskMethodBuilder<int> methodBuilder;
            public int state;
            public TaskAwaiter awaiter;

            public void MoveNext()
            {
                if (state == 0)
                {
                    awaiter = Task.Delay(10000).GetAwaiter();
                    if (awaiter.IsCompleted)
                    {
                        state = 1;
                        goto state1;
                    }
                    else
                    {
                        state = 1;
                        methodBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref this);
                    }
                    return;
                }

state1:
                if(state == 1)
                {
                    awaiter.GetResult();
                    methodBuilder.SetResult(42);
                }
            }

            public void SetStateMachine(IAsyncStateMachine stateMachine)
            {
                methodBuilder.SetStateMachine(stateMachine);
            }
        }
    }
}
