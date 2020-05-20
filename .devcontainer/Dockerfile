FROM cecropia/dotnet-with-zip:3.1

ARG USERNAME=vscode
ARG USER_UID=1000
ARG USER_GID=$USER_UID

ENV DEBIAN_FRONTEND=noninteractive

# update repositories of base ubuntu image
RUN sed -i -e 's|disco|eoan|g' /etc/apt/sources.list

RUN apt-get update \
    && apt-get -y install git iproute2 procps apt-transport-https gnupg2 curl lsb-release jq nano \
    && groupadd --gid $USER_GID $USERNAME \
    && useradd -s /bin/bash --uid $USER_UID --gid $USER_GID -m $USERNAME \
    && apt-get install -y sudo \
    && echo $USERNAME ALL=\(root\) NOPASSWD:ALL > /etc/sudoers.d/$USERNAME\
    && chmod 0440 /etc/sudoers.d/$USERNAME \
    && apt-get autoremove -y \
    && apt-get clean -y

ENV PATH="/home/vscode/.dotnet/tools:${PATH}"
ENV EDITOR="nano"

ENV DEBIAN_FRONTEND=dialog
